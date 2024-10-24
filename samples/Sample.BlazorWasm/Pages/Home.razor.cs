using Microsoft.AspNetCore.Components;

namespace Sample.BlazorWasm.Pages;

public partial class Home
{
    public enum TaskComponentType { None, Create, Edit, Delete }

    TaskComponentType _showComponent = TaskComponentType.None;
    TaskEntity[] _tasks = [];
    TaskEntity? _selectedTask = null;

    protected override void OnInitialized() => _tasks = _service.GetTasks();

    private void OnCreate()
    {
        _selectedTask = null;
        _showComponent = TaskComponentType.Create;
    }

    private void OnUpdate(TaskEntity selectedTask)
    {
        _selectedTask = selectedTask;
        _showComponent = TaskComponentType.Edit;
    }

    private void OnDelete(TaskEntity selectedTask)
    {
        _selectedTask = selectedTask;
        _showComponent = TaskComponentType.Delete;
    }

    private void OnTasksUpdated()
    {
        if (_showComponent is TaskComponentType.Delete) _showComponent = TaskComponentType.None;
        _tasks = _service.GetTasks();
    }

    private RenderFragment? GetSelectedComponent() =>
        _showComponent switch
        {
            TaskComponentType.Create => RenderCreate(),
            TaskComponentType.Edit => RenderEdit(),
            TaskComponentType.Delete => RenderDelete(),
            _ => null
        };
}
