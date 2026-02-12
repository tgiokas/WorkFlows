using DocumentWorkflow.Domain.Entities;

namespace DocumentWorkflow.Domain.Interfaces;

public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByStatusAsync(string status);
    Task<IEnumerable<Document>> GetBySubmitterAsync(string submitterId);
    Task<IEnumerable<Document>> GetByDocumentTypeAsync(string documentType);
}

public interface IWorkflowDefinitionRepository : IRepository<WorkflowDefinition>
{
    Task<WorkflowDefinition?> GetActiveByDocumentTypeAsync(string documentType);
    Task<IEnumerable<WorkflowDefinition>> GetByDocumentTypeAsync(string documentType);
    Task<WorkflowDefinition?> GetLatestVersionAsync(string name);
}

public interface IWorkflowInstanceRepository : IRepository<WorkflowInstance>
{
    Task<IEnumerable<WorkflowInstance>> GetByDocumentIdAsync(Guid documentId);
    Task<IEnumerable<WorkflowInstance>> GetRunningInstancesAsync();
    Task<WorkflowInstance?> GetWithStepsAsync(Guid instanceId);
}

public interface IWorkflowStepRepository : IRepository<WorkflowStep>
{
    Task<IEnumerable<WorkflowStep>> GetByInstanceIdAsync(Guid instanceId);
    Task<WorkflowStep?> GetCurrentStepAsync(Guid instanceId);
}

public interface IApprovalTaskRepository : IRepository<ApprovalTask>
{
    Task<IEnumerable<ApprovalTask>> GetByAssigneeAsync(string assignee);
    Task<IEnumerable<ApprovalTask>> GetPendingTasksAsync(string assignee);
    Task<IEnumerable<ApprovalTask>> GetByDocumentIdAsync(Guid documentId);
}

public interface IApprovalPolicyRepository : IRepository<ApprovalPolicy>
{
    Task<IEnumerable<ApprovalPolicy>> GetActiveByDocumentTypeAsync(string documentType);
    Task<ApprovalPolicy?> GetMatchingPolicyAsync(string documentType, string department, decimal? amount);
}
