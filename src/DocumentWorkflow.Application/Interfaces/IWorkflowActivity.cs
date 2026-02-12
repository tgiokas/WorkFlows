using DocumentWorkflow.Domain.Entities;

namespace DocumentWorkflow.Application.Activities;

/// <summary>
/// Base interface for workflow activities (service tasks)
/// </summary>
public interface IWorkflowActivity
{
    /// <summary>
    /// The activity name that maps to BPMN serviceTask name
    /// </summary>
    string ActivityName { get; }
    
    /// <summary>
    /// Execute the activity logic
    /// </summary>
    Task ExecuteAsync(WorkflowInstance instance, Document document);
}
