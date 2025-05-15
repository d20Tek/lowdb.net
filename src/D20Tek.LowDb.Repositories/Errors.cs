using D20Tek.Functional;

namespace D20Tek.LowDb.Repositories;

internal static class Errors
{
    public static Result<T> NotFoundError<T>(object id) where T : notnull =>
        Result<T>.Failure(Error.NotFound("Entity.NotFound", $"Entity with id={id} not found."));

    public static Result<T> AddFailedError<T>(object entity) where T : notnull =>
        Result<T>.Failure(Error.Conflict("Entity.AddFailed", $"Entity cannot be added to the database: {entity}."));

    public static Result<T> RemoveFailedError<T>(object entity) where T : notnull =>
        Result<T>.Failure(Error.Failure("Entity.RemoveFailed", $"Entity cannot be removed to the database: {entity}."));
}
