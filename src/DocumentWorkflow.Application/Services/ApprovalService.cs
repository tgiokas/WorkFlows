using DocumentWorkflow.Application.DTOs;
using DocumentWorkflow.Application.Interfaces;
using DocumentWorkflow.Domain.Interfaces;

namespace DocumentWorkflow.Application.Services;

public class ApprovalService : IApprovalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWorkflowEngineService _workflowEngine;

    public ApprovalService(IUnitOfWork unitOfWork, IWorkflowEngineService workflowEngine)
    {
        _unitOfWork = unitOfWork;
        _workflowEngine = workflowEngine;
    }

    public async Task<IEnumerable<ApprovalTaskDto>> GetPendingTasksForUserAsync(string userId)
    {
        var tasks = await _unitOfWork.ApprovalTasks.GetPendingTasksAsync(userId);
        var dtos = new List<ApprovalTaskDto>();

        foreach (var task in tasks)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(task.DocumentId);
            dtos.Add(new ApprovalTaskDto
            {
                Id = task.Id,
                DocumentId = task.DocumentId,
                DocumentTitle = document?.Title ?? "Unknown",
                TaskName = task.TaskName,
                AssignedTo = task.AssignedTo,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate
            });
        }

        return dtos;
    }

    public async Task<bool> CompleteApprovalTaskAsync(CompleteApprovalTaskDto dto)
    {
        var task = await _unitOfWork.ApprovalTasks.GetByIdAsync(dto.TaskId);
        if (task == null || task.Status != "Pending")
            return false;

        // Update task
        task.Status = dto.Decision == "approve" ? "Approved" : 
                     dto.Decision == "reject" ? "Rejected" : "ChangesRequested";
        task.Comments = dto.Comments;
        task.CompletedAt = DateTime.UtcNow;
        await _unitOfWork.ApprovalTasks.UpdateAsync(task);

        // Update document based on decision
        var document = await _unitOfWork.Documents.GetByIdAsync(task.DocumentId);
        if (document != null)
        {
            if (dto.Decision == "reject")
            {
                document.Status = "Rejected";
            }
            else if (dto.Decision == "changes")
            {
                document.Status = "ChangesRequested";
            }
            else if (dto.Decision == "approve")
            {
                document.Status = "Approved";
            }
            await _unitOfWork.Documents.UpdateAsync(document);
        }

        await _unitOfWork.SaveChangesAsync();

        // Continue workflow if approved
        if (dto.Decision == "approve")
        {
            var instance = await _unitOfWork.WorkflowInstances
                .FirstOrDefaultAsync(wi => wi.Id == task.WorkflowInstanceId);
            
            if (instance != null)
            {
                await _workflowEngine.ExecuteNextStepAsync(instance.Id);
            }
        }

        return true;
    }

    public async Task<IEnumerable<ApprovalTaskDto>> GetTasksForDocumentAsync(Guid documentId)
    {
        var tasks = await _unitOfWork.ApprovalTasks.GetByDocumentIdAsync(documentId);
        var document = await _unitOfWork.Documents.GetByIdAsync(documentId);

        return tasks.Select(task => new ApprovalTaskDto
        {
            Id = task.Id,
            DocumentId = task.DocumentId,
            DocumentTitle = document?.Title ?? "Unknown",
            TaskName = task.TaskName,
            AssignedTo = task.AssignedTo,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate
        });
    }
}
