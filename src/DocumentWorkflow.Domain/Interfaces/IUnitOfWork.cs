namespace DocumentWorkflow.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern to manage transactions across repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IDocumentRepository Documents { get; }
    IWorkflowDefinitionRepository WorkflowDefinitions { get; }
    IWorkflowInstanceRepository WorkflowInstances { get; }
    IWorkflowStepRepository WorkflowSteps { get; }
    IApprovalTaskRepository ApprovalTasks { get; }
    IApprovalPolicyRepository ApprovalPolicies { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
