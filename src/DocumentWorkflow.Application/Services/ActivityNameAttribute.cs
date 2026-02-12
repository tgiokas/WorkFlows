namespace DocumentWorkflow.Application.Activities;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ActivityNameAttribute : Attribute
{
    public string Name { get; }

    public ActivityNameAttribute(string name)
    {
        Name = name;
    }
}