namespace DocumentWorkflow.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // Invoice, Contract, Report, etc.
    public string Department { get; set; } = string.Empty;
    public decimal? Amount { get; set; } // For invoice approval routing
    public string Status { get; set; } = "Draft"; // Draft, Submitted, InApproval, Approved, Rejected, Published
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    
    // Metadata as JSON
    public string Metadata { get; set; } = "{}";
    
    // Navigation properties
    public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; } = new List<WorkflowInstance>();
}
