using DocumentWorkflow.Application.Activities;
using DocumentWorkflow.Domain.Entities;

namespace DocumentWorkflow.Application.Activities;

/// <summary>
/// Activity: Publishes approved document
/// Maps to BPMN: <serviceTask name="Publish Document">
/// </summary>
public class PublishDocumentActivity : IWorkflowActivity
{
    public string ActivityName => "Publish Document";

    public async Task ExecuteAsync(WorkflowInstance instance, Document document)
    {
        if (document == null)
            throw new InvalidOperationException("Document not found");

        // Publishing logic
        document.Status = "Published";
        document.PublishedAt = DateTime.UtcNow;
        
        // Additional publishing logic could include:
        // - Lock the document version
        // - Generate PDF
        // - Send notifications
        // - Update search index
        // - Archive old versions
        
        Console.WriteLine($"[PublishDocumentActivity] Document {document.Id} published successfully");
        
        await Task.CompletedTask;
    }
}
