namespace DocumentWorkflow.Application.Activities;

/// <summary>
/// Factory Pattern: Creates appropriate activity based on task name
/// </summary>
public interface IActivityFactory
{
    IWorkflowActivity? CreateActivity(string taskName);
    void RegisterActivity<T>() where T : IWorkflowActivity, new();
}

/// <summary>
/// Implementation using Service Locator + Factory patterns
/// </summary>
public class ActivityFactory : IActivityFactory
{
    private readonly Dictionary<string, Type> _activityTypes = new();
    private readonly IServiceProvider? _serviceProvider;

    public ActivityFactory(IServiceProvider? serviceProvider = null)
    {
        _serviceProvider = serviceProvider;
        
        // Auto-register all activities
        RegisterActivity<ValidateDocumentActivity>();
        RegisterActivity<PublishDocumentActivity>();
    }

    public void RegisterActivity<T>() where T : IWorkflowActivity, new()
    {
        var instance = new T();
        _activityTypes[instance.ActivityName] = typeof(T);
    }

    public IWorkflowActivity? CreateActivity(string taskName)
    {
        // Try exact match first
        if (_activityTypes.TryGetValue(taskName, out var activityType))
        {
            return CreateInstance(activityType);
        }

        // Try fuzzy match (contains, case-insensitive)
        var fuzzyMatch = _activityTypes.FirstOrDefault(kvp => 
            taskName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(fuzzyMatch.Key))
        {
            return CreateInstance(fuzzyMatch.Value);
        }

        return null;
    }

    private IWorkflowActivity? CreateInstance(Type activityType)
    {
        // Use DI if available (for activities with dependencies)
        if (_serviceProvider != null)
        {
            return _serviceProvider.GetService(activityType) as IWorkflowActivity;
        }

        // Otherwise, create with parameterless constructor
        return Activator.CreateInstance(activityType) as IWorkflowActivity;
    }
}
