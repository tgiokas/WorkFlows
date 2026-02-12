namespace DocumentWorkflow.Domain.Entities;

/// <summary>
/// Defines approval routing rules based on document attributes
/// </summary>
public class ApprovalPolicy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    
    // Conditions as JSON expression
    // Example: { "amount": { "greaterThan": 1000 } }
    public string Conditions { get; set; } = "{}";
    
    // Approvers in order (comma-separated emails or roles)
    public string Approvers { get; set; } = string.Empty;
    
    public bool RequireAllApprovals { get; set; } = true; // Sequential vs parallel
    public int Priority { get; set; } = 0; // Higher priority policies are checked first
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
