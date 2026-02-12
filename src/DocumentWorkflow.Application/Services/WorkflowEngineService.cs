using System.Xml.Linq;
using DocumentWorkflow.Application.DTOs;
using DocumentWorkflow.Application.Interfaces;
using DocumentWorkflow.Application.Activities;
using DocumentWorkflow.Domain.Entities;
using DocumentWorkflow.Domain.Interfaces;

namespace DocumentWorkflow.Application.Services;
public class WorkflowEngineService : IWorkflowEngineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IActivityFactory _activityFactory;
    //private readonly ITaskConfigurationService _taskConfigService;

    public WorkflowEngineService(
        IUnitOfWork unitOfWork, 
        IActivityFactory activityFactory)
    //    ITaskConfigurationService taskConfigService)
    {
        _unitOfWork = unitOfWork;
        _activityFactory = activityFactory;
       // _taskConfigService = taskConfigService;
    }

    public async Task<WorkflowInstanceDto> StartWorkflowAsync(Guid workflowDefinitionId, Guid documentId)
    {
        var workflowDef = await _unitOfWork.WorkflowDefinitions.GetByIdAsync(workflowDefinitionId);
        if (workflowDef == null)
            throw new InvalidOperationException("Workflow definition not found");

        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid(),
            WorkflowDefinitionId = workflowDefinitionId,
            DocumentId = documentId,
            CurrentStepId = "", // Empty - will find start event
            Status = "Running",
            Variables = "{}",
            StartedAt = DateTime.UtcNow
        };

        await _unitOfWork.WorkflowInstances.AddAsync(instance);
        await _unitOfWork.SaveChangesAsync();

        // Execute first step automatically
        await ExecuteNextStepAsync(instance.Id);

        return MapToDto(instance);
    }

    public async Task<bool> ExecuteNextStepAsync(Guid instanceId)
    {
        var instance = await _unitOfWork.WorkflowInstances.GetByIdAsync(instanceId);
        if (instance == null || instance.Status != "Running")
            return false;

        var workflowDef = await _unitOfWork.WorkflowDefinitions.GetByIdAsync(instance.WorkflowDefinitionId);
        if (workflowDef == null) return false;

        // Simple BPMN parser - finds next step
        var nextStep = await FindNextStepAsync(workflowDef.BpmnXml, instance.CurrentStepId);
        
        if (nextStep == null)
        {
            // Workflow completed
            instance.Status = "Completed";
            instance.CompletedAt = DateTime.UtcNow;
            await _unitOfWork.WorkflowInstances.UpdateAsync(instance);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Log this step
        var step = new WorkflowStep
        {
            Id = Guid.NewGuid(),
            WorkflowInstanceId = instance.Id,
            StepId = nextStep.Id,
            StepName = nextStep.Name,
            StepType = nextStep.Type,
            Status = "Completed",
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };
        await _unitOfWork.WorkflowSteps.AddAsync(step);

        // Execute step based on type (handle both lowercase and capitalized)
        var stepType = nextStep.Type.ToLower();
        
        if (stepType == "usertask")
        {
            // Create approval task
            var document = await _unitOfWork.Documents.GetByIdAsync(instance.DocumentId);
            var approvalTask = new ApprovalTask
            {
                Id = Guid.NewGuid(),
                WorkflowInstanceId = instance.Id,
                DocumentId = instance.DocumentId,
                TaskName = nextStep.Name,
                AssignedTo = "manager@company.com", // Simplified - would use policy
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(3)
            };
            await _unitOfWork.ApprovalTasks.AddAsync(approvalTask);
            
            // Wait for user action - don't advance yet
            instance.CurrentStepId = nextStep.Id;
        }
        else if (stepType == "servicetask")
        {
            // Execute service task
            await ExecuteServiceTaskAsync(nextStep.Name, instance);
            instance.CurrentStepId = nextStep.Id;
            
            // Auto-advance for service tasks
            await _unitOfWork.SaveChangesAsync();
            await ExecuteNextStepAsync(instanceId);
            return true;
        }
        else if (stepType == "endevent")
        {
            instance.Status = "Completed";
            instance.CompletedAt = DateTime.UtcNow;
            
            // Update document status
            var document = await _unitOfWork.Documents.GetByIdAsync(instance.DocumentId);
            if (document != null)
            {
                document.Status = "Published";
                document.PublishedAt = DateTime.UtcNow;
                await _unitOfWork.Documents.UpdateAsync(document);
            }
        }

        await _unitOfWork.WorkflowInstances.UpdateAsync(instance);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<WorkflowInstanceDto?> GetWorkflowInstanceAsync(Guid instanceId)
    {
        var instance = await _unitOfWork.WorkflowInstances.GetByIdAsync(instanceId);
        return instance == null ? null : MapToDto(instance);
    }

    private async Task<BpmnStep?> FindNextStepAsync(string bpmnXml, string currentStepId)
    {
        try
        {
            var doc = XDocument.Parse(bpmnXml);
            
            // Get BPMN namespace explicitly
            XNamespace ns = "http://www.omg.org/spec/BPMN/20100524/MODEL";

            // If no current step or empty, start from the beginning
            if (string.IsNullOrEmpty(currentStepId))
            {
                var startEvent = doc.Descendants(ns + "startEvent").FirstOrDefault();
                if (startEvent != null)
                {
                    var firstTaskId = doc.Descendants(ns + "sequenceFlow")
                        .FirstOrDefault(sf => sf.Attribute("sourceRef")?.Value == startEvent.Attribute("id")?.Value)
                        ?.Attribute("targetRef")?.Value;

                    if (firstTaskId != null)
                    {
                        var firstTask = doc.Descendants().FirstOrDefault(e => e.Attribute("id")?.Value == firstTaskId);
                        if (firstTask != null)
                        {
                            return new BpmnStep
                            {
                                Id = firstTaskId,
                                Name = firstTask.Attribute("name")?.Value ?? "Unnamed",
                                Type = firstTask.Name.LocalName
                            };
                        }
                    }
                }
                return null;
            }

            // Find current element
            var currentElement = doc.Descendants()
                .FirstOrDefault(e => e.Attribute("id")?.Value == currentStepId);

            if (currentElement == null)
            {
                // If current element not found, try to start from beginning
                var startEvent = doc.Descendants(ns + "startEvent").FirstOrDefault();
                if (startEvent != null)
                {
                    var firstTaskId = doc.Descendants(ns + "sequenceFlow")
                        .FirstOrDefault(sf => sf.Attribute("sourceRef")?.Value == startEvent.Attribute("id")?.Value)
                        ?.Attribute("targetRef")?.Value;

                    if (firstTaskId != null)
                    {
                        var firstTask = doc.Descendants().FirstOrDefault(e => e.Attribute("id")?.Value == firstTaskId);
                        if (firstTask != null)
                        {
                            return new BpmnStep
                            {
                                Id = firstTaskId,
                                Name = firstTask.Attribute("name")?.Value ?? "Unnamed",
                                Type = firstTask.Name.LocalName
                            };
                        }
                    }
                }
                return null;
            }

            // Find outgoing sequence flow
            var outgoingFlow = doc.Descendants(ns + "sequenceFlow")
                .FirstOrDefault(sf => sf.Attribute("sourceRef")?.Value == currentStepId);

            if (outgoingFlow == null) return null;

            var targetId = outgoingFlow.Attribute("targetRef")?.Value;
            if (targetId == null) return null;

            var targetElement = doc.Descendants().FirstOrDefault(e => e.Attribute("id")?.Value == targetId);
            if (targetElement == null) return null;

            return new BpmnStep
            {
                Id = targetId,
                Name = targetElement.Attribute("name")?.Value ?? "Unnamed",
                Type = targetElement.Name.LocalName
            };
        }
        catch
        {
            return null;
        }
    }

    private async Task ExecuteServiceTaskAsync(string taskName, WorkflowInstance instance)
    {
        var document = await _unitOfWork.Documents.GetByIdAsync(instance.DocumentId);
        if (document == null) return;

        IWorkflowActivity? activity = null;

        //// Option 1: Check if task is configured in database (user-mapped)
        //var taskConfig = await _taskConfigService.GetConfigurationAsync(
        //    instance.WorkflowDefinitionId, 
        //    instance.CurrentStepId);

        //if (taskConfig != null)
        //{
        //    // User configured this task - use their mapping
        //    activity = _activityFactory.CreateActivity(taskConfig.ActivityType);
        //    Console.WriteLine($"[WorkflowEngine] Using configured activity '{taskConfig.ActivityType}' for task '{taskName}'");
        //}
        //else
        //{
        // Option 2: Fall back to name-based matching
        activity = _activityFactory.CreateActivity(taskName);
        Console.WriteLine($"[WorkflowEngine] Using name-based activity for task '{taskName}'");
        //}

        if (activity != null)
        {
            // Strategy Pattern: Execute the activity (polymorphism)
            await activity.ExecuteAsync(instance, document);
            Console.WriteLine($"[WorkflowEngine] Executed activity successfully");
        }
        else
        {
            // Null Object Pattern: Handle missing activities gracefully
            Console.WriteLine($"[WorkflowEngine] ⚠️ Warning: No activity found for task '{taskName}'");
            Console.WriteLine($"[WorkflowEngine] Please configure this task in the workflow settings");
        }

        await _unitOfWork.Documents.UpdateAsync(document);
    }

    private static WorkflowInstanceDto MapToDto(WorkflowInstance instance)
    {
        return new WorkflowInstanceDto
        {
            Id = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            DocumentId = instance.DocumentId,
            CurrentStepId = instance.CurrentStepId,
            Status = instance.Status,
            StartedAt = instance.StartedAt,
            CompletedAt = instance.CompletedAt
        };
    }

    private class BpmnStep
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
