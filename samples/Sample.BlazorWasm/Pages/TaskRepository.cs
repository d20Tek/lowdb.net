using D20Tek.LowDb;

namespace Sample.BlazorWasm.Pages;

internal class TaskRepository(LowDbAsync<TasksDocument> db)
{
    private readonly LowDbAsync<TasksDocument> _db = db;

    public async Task<TaskEntity[]> GetTasks()
    {
        var taskDoc = await _db.Get();
        return [.. taskDoc.Tasks];
    }

    public async Task<bool> CreateTask(string name, bool isCompleted = false)
    {
        if (string.IsNullOrEmpty(name)) return false;

        return await TryOperation(async () =>
        {
            await _db.Update(doc =>
            {
                doc.LastId = doc.GetNextId();
                doc.Tasks.Add(new TaskEntity { Id = doc.LastId, Name = name, IsCompleted = isCompleted });
            });
            return true;
        });
    }

    public async Task<bool> UpdateTask(TaskEntity updatedTask)
    {
        return await TryOperation(async () =>
        {
            var result = false;
            await _db.Update(x =>
            {
                var task = x.Tasks.FirstOrDefault(x => x.Id == updatedTask.Id);
                if (task is not null)
                {
                    task.Name = updatedTask.Name;
                    task.IsCompleted = updatedTask.IsCompleted;

                    result = true;
                }
            });
            return result;
        });
    }

    public async Task<bool> DeleteTask(int id)
    {
        return await TryOperation(async () =>
        {
            await _db.Update(doc =>
            {
                doc.Tasks.RemoveAll(x => x.Id == id);
            });
            return true;
        });
    }

    private static async Task<bool> TryOperation(Func<Task<bool>> operation)
    {
        try { return await operation(); }
        catch (Exception) { return false; }
    }
}
