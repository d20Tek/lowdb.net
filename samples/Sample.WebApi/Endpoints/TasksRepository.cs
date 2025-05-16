using D20Tek.LowDb;
using D20Tek.LowDb.Repositories;

namespace Sample.WebApi.Endpoints;

internal interface ITasksRepository : IRepositoryAsync<TaskEntity>;

internal class TasksRepository : LowDbAsyncRepository<TaskEntity, TasksDocument>, ITasksRepository
{
    public TasksRepository(LowDbAsync<TasksDocument> db)
        : base(db, x => x.Tasks)
    {
    }
}
