namespace DocumentWorkflow.Domain.Entities;

/// <summary>
/// Represents a BPMN workflow definition stored in the system
/// </summary>
public class WorkflowDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // Which document types use this workflow
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    
    // The actual BPMN XML content
    public string BpmnXml { get; set; } = string.Empty;
    
    // Parsed workflow structure as JSON (for quick access)
    public string ParsedStructure { get; set; } = "{}";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; } = new List<WorkflowInstance>();
}
