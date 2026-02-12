namespace DocumentWorkflow.Domain.Entities;

/// <summary>
/// Represents an approval task assigned to a user
/// </summary>
public class ApprovalTask
{
    public Guid Id { get; set; }
    public Guid WorkflowInstanceId { get; set; }
    public Guid DocumentId { get; set; }
    
    public string TaskName { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty; // User email or ID
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, ChangesRequested
    
    public string Comments { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? DueDate { get; set; }
    
    // Navigation properties
    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;
    public virtual Document Document { get; set; } = null!;
}
