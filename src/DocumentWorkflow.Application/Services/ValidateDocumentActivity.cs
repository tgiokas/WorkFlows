using DocumentWorkflow.Application.Activities;
using DocumentWorkflow.Domain.Entities;

namespace DocumentWorkflow.Application.Activities;

/// <summary>
/// Activity: Validates document before approval
/// Supports multiple BPMN task names via attributes
/// </summary>
[ActivityName("Validate Document")]
[ActivityName("Check Document")]
[ActivityName("Verify Submission")]
[ActivityName("Validate Invoice")]
[ActivityName("Validate Contract")]
public class ValidateDocumentActivity : IWorkflowActivity
{
    public string ActivityName => "Validate Document";

    public async Task ExecuteAsync(WorkflowInstance instance, Document document)
    {
        // Validation logic
        if (document == null)
            throw new InvalidOperationException("Document not found");

        if (string.IsNullOrEmpty(document.Title))
            throw new InvalidOperationException("Document title is required");

        if (string.IsNullOrEmpty(document.DocumentType))
            throw new InvalidOperationException("Document type is required");

        // Update document status
        document.Status = "InApproval";
        
        // Log activity execution
        Console.WriteLine($"[ValidateDocumentActivity] Document {document.Id} validated successfully");
        
        await Task.CompletedTask;
    }
}
