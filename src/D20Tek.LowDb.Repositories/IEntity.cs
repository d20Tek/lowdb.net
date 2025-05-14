namespace D20Tek.LowDb.Repositories;

public interface IEntity<TId> where TId : notnull, IEquatable<TId>
{
    public TId Id { get; }

    public void SetId(TId id);
}
