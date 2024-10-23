//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb;

namespace Sample.BlazorWasm.Pages;

internal class TaskRepository
{
    private readonly LowDb<TasksDocument> _db;

    public TaskRepository(LowDb<TasksDocument> db) => _db = db;

    public TaskEntity[] GetTasks()
    {
        var taskDoc = _db.Get();
        return [.. taskDoc.Tasks];
    }

    public bool CreateTask(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;

        try
        {
            _db.Update(doc =>
            {
                doc.LastId = doc.GetNextId();
                doc.Tasks.Add(new TaskEntity { Id = doc.LastId, Name = name });
            });

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateTask(TaskEntity updatedTask)
    {
        try
        {
            var result = false;
            _db.Update(x =>
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
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteTask(int id)
    {
        try
        {
            _db.Update(doc =>
            {
                doc.Tasks.RemoveAll(x => x.Id == id);
            });

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
