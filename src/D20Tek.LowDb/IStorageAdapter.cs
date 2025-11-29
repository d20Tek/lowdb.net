namespace D20Tek.LowDb;

public interface IStorageAdapter<T>
    where T : class
{
    T? Read();

    void Write(T data);
}
