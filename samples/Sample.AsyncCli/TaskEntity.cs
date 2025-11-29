namespace Sample.AsyncCli;

internal class TaskEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
    
    public bool IsCompleted { get; set; }

    public EntityState State { get; set; } = EntityState.Active;

    public override string ToString() => $"Id: {Id}, Name: {Name}, IsCompleted: {IsCompleted}, State: {State}";
}

public enum EntityState
{
    Active,
    Inactive,
    Completed
}
