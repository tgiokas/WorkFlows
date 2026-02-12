namespace DocumentWorkflow.Application.DTOs;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class CreateDocumentDto
{
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
}

public class WorkflowDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public string BpmnXml { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateWorkflowDefinitionDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string BpmnXml { get; set; } = string.Empty;
}

public class WorkflowInstanceDto
{
    public Guid Id { get; set; }
    public Guid WorkflowDefinitionId { get; set; }
    public Guid DocumentId { get; set; }
    public string CurrentStepId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class ApprovalTaskDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
}

public class CompleteApprovalTaskDto
{
    public Guid TaskId { get; set; }
    public string Decision { get; set; } = string.Empty; // approve, reject, changes
    public string Comments { get; set; } = string.Empty;
}

public class BpmnValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
