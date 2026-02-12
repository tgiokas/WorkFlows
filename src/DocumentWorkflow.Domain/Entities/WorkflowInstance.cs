namespace DocumentWorkflow.Domain.Entities;

/// <summary>
/// Represents a running instance of a workflow for a specific document
/// </summary>
public class WorkflowInstance
{
    public Guid Id { get; set; }
    public Guid WorkflowDefinitionId { get; set; }
    public Guid DocumentId { get; set; }
    
    public string CurrentStepId { get; set; } = string.Empty; // BPMN element ID we're currently at
    public string Status { get; set; } = "Running"; // Running, Completed, Failed, Cancelled
    
    // Workflow variables/context as JSON
    public string Variables { get; set; } = "{}";
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public virtual WorkflowDefinition WorkflowDefinition { get; set; } = null!;
    public virtual Document Document { get; set; } = null!;
    public virtual ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
    public virtual ICollection<ApprovalTask> ApprovalTasks { get; set; } = new List<ApprovalTask>();
}
