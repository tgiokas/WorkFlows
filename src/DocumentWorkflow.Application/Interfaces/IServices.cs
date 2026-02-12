using DocumentWorkflow.Application.DTOs;

namespace DocumentWorkflow.Application.Interfaces;

public interface IDocumentService
{
    Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto dto);
    Task<DocumentDto?> GetDocumentByIdAsync(Guid id);
    Task<IEnumerable<DocumentDto>> GetDocumentsByStatusAsync(string status);
    Task<bool> SubmitDocumentForApprovalAsync(Guid documentId);
}

public interface IWorkflowService
{
    Task<WorkflowDefinitionDto> SaveWorkflowDefinitionAsync(CreateWorkflowDefinitionDto dto);
    Task<IEnumerable<WorkflowDefinitionDto>> GetAllWorkflowsAsync();
    Task<WorkflowDefinitionDto?> GetWorkflowByIdAsync(Guid id);
    Task<WorkflowDefinitionDto?> GetActiveWorkflowForDocumentTypeAsync(string documentType);
    Task<bool> DeleteWorkflowAsync(Guid id);
    Task<BpmnValidationResult> ValidateBpmnAsync(string bpmnXml);
    Task<object> ParseBpmnStructureAsync(string bpmnXml);
}

public interface IWorkflowEngineService
{
    Task<WorkflowInstanceDto> StartWorkflowAsync(Guid workflowDefinitionId, Guid documentId);
    Task<bool> ExecuteNextStepAsync(Guid instanceId);
    Task<WorkflowInstanceDto?> GetWorkflowInstanceAsync(Guid instanceId);
}

public interface IApprovalService
{
    Task<IEnumerable<ApprovalTaskDto>> GetPendingTasksForUserAsync(string userId);
    Task<bool> CompleteApprovalTaskAsync(CompleteApprovalTaskDto dto);
    Task<IEnumerable<ApprovalTaskDto>> GetTasksForDocumentAsync(Guid documentId);
}
