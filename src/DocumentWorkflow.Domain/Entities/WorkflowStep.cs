namespace DocumentWorkflow.Domain.Entities;

/// <summary>
/// Audit trail of each step executed in a workflow instance
/// </summary>
public class WorkflowStep
{
    public Guid Id { get; set; }
    public Guid WorkflowInstanceId { get; set; }
    
    public string StepId { get; set; } = string.Empty; // BPMN element ID
    public string StepName { get; set; } = string.Empty;
    public string StepType { get; set; } = string.Empty; // StartEvent, ServiceTask, UserTask, Gateway, EndEvent
    
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Skipped
    public string Result { get; set; } = string.Empty; // For gateways: which path was taken
    
    // Additional data from step execution
    public string ExecutionData { get; set; } = "{}";
    public string ErrorMessage { get; set; } = string.Empty;
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    // Navigation property
    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;
}
