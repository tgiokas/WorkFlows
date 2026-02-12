using System.Xml.Linq;
using DocumentWorkflow.Application.DTOs;
using DocumentWorkflow.Application.Interfaces;
using DocumentWorkflow.Domain.Entities;
using DocumentWorkflow.Domain.Interfaces;

namespace DocumentWorkflow.Application.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IUnitOfWork _unitOfWork;

    public WorkflowService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkflowDefinitionDto> SaveWorkflowDefinitionAsync(CreateWorkflowDefinitionDto dto)
    {
        // Validate BPMN first
        var validation = await ValidateBpmnAsync(dto.BpmnXml);
        if (!validation.IsValid)
            throw new InvalidOperationException($"Invalid BPMN: {string.Join(", ", validation.Errors)}");

        // Check for existing workflow with same name
        var existingWorkflow = await _unitOfWork.WorkflowDefinitions.GetLatestVersionAsync(dto.Name);
        
        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            DocumentType = dto.DocumentType,
            Version = existingWorkflow != null ? existingWorkflow.Version + 1 : 1,
            IsActive = true,
            BpmnXml = dto.BpmnXml,
            ParsedStructure = "{}",  // Could parse here
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "system"
        };

        // Deactivate old versions
        if (existingWorkflow != null)
        {
            existingWorkflow.IsActive = false;
            await _unitOfWork.WorkflowDefinitions.UpdateAsync(existingWorkflow);
        }

        await _unitOfWork.WorkflowDefinitions.AddAsync(workflow);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(workflow);
    }

    public async Task<IEnumerable<WorkflowDefinitionDto>> GetAllWorkflowsAsync()
    {
        var workflows = await _unitOfWork.WorkflowDefinitions.GetAllAsync();
        return workflows.Select(MapToDto).OrderByDescending(w => w.CreatedAt);
    }

    public async Task<WorkflowDefinitionDto?> GetWorkflowByIdAsync(Guid id)
    {
        var workflow = await _unitOfWork.WorkflowDefinitions.GetByIdAsync(id);
        return workflow == null ? null : MapToDto(workflow);
    }

    public async Task<WorkflowDefinitionDto?> GetActiveWorkflowForDocumentTypeAsync(string documentType)
    {
        var workflow = await _unitOfWork.WorkflowDefinitions.GetActiveByDocumentTypeAsync(documentType);
        return workflow == null ? null : MapToDto(workflow);
    }

    public async Task<bool> DeleteWorkflowAsync(Guid id)
    {
        var workflow = await _unitOfWork.WorkflowDefinitions.GetByIdAsync(id);
        if (workflow == null) return false;

        await _unitOfWork.WorkflowDefinitions.DeleteAsync(workflow);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public Task<BpmnValidationResult> ValidateBpmnAsync(string bpmnXml)
    {
        var result = new BpmnValidationResult { IsValid = true };

        try
        {
            var doc = XDocument.Parse(bpmnXml);
            //var ns = doc.Root?.GetDefaultNamespace();

            // Get BPMN namespace explicitly
            XNamespace ns = "http://www.omg.org/spec/BPMN/20100524/MODEL";

            //if (ns == null || !ns.NamespaceName.Contains("bpmn"))
            //{
            //    result.IsValid = false;
            //    result.Errors.Add("Invalid BPMN namespace");
            //    return Task.FromResult(result);
            //}

            // Check for at least one process
            var processes = doc.Descendants(ns + "process");
            if (!processes.Any())
            {
                result.IsValid = false;
                result.Errors.Add("BPMN must contain at least one process");
            }

            // Check for start event
            var startEvents = doc.Descendants(ns + "startEvent");
            if (!startEvents.Any())
            {
                result.Warnings.Add("No start event found");
            }

            // Check for end event
            var endEvents = doc.Descendants(ns + "endEvent");
            if (!endEvents.Any())
            {
                result.Warnings.Add("No end event found");
            }
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"XML parsing error: {ex.Message}");
        }

        return Task.FromResult(result);
    }

    public Task<object> ParseBpmnStructureAsync(string bpmnXml)
    {
        try
        {
            var doc = XDocument.Parse(bpmnXml);
            var ns = doc.Root?.GetDefaultNamespace();

            var structure = new
            {
                processes = doc.Descendants(ns + "process").Select(p => new
                {
                    id = p.Attribute("id")?.Value,
                    name = p.Attribute("name")?.Value,
                    startEvents = p.Descendants(ns + "startEvent").Count(),
                    endEvents = p.Descendants(ns + "endEvent").Count(),
                    tasks = p.Descendants(ns + "serviceTask").Count() + p.Descendants(ns + "userTask").Count(),
                    gateways = p.Descendants(ns + "exclusiveGateway").Count()
                }).ToList()
            };

            return Task.FromResult<object>(structure);
        }
        catch (Exception ex)
        {
            return Task.FromResult<object>(new { error = ex.Message });
        }
    }

    private static WorkflowDefinitionDto MapToDto(WorkflowDefinition workflow)
    {
        return new WorkflowDefinitionDto
        {
            Id = workflow.Id,
            Name = workflow.Name,
            Description = workflow.Description,
            DocumentType = workflow.DocumentType,
            Version = workflow.Version,
            IsActive = workflow.IsActive,
            BpmnXml = workflow.BpmnXml,
            CreatedAt = workflow.CreatedAt
        };
    }
}
