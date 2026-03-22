using MongoDB.Driver;
using Robotico.Domain;
using Robotico.Result.Errors;

namespace Robotico.Repository.MongoDb;

/// <summary>
/// Maps <see cref="MongoException"/> responses from the MongoDB driver to <see cref="Robotico.Result.Result"/>
/// values used by <see cref="MongoDbRepository{TEntity,TId}"/>.
/// </summary>
/// <remarks>
/// Centralizing classification keeps repository methods linear and matches the testability pattern used by
/// <c>CosmosDbRepositoryCosmosExceptionRouter</c> in <c>Robotico.Repository.CosmosDb</c>.
/// </remarks>
internal static class MongoDbRepositoryMongoExceptionRouter
{
    /// <summary>
    /// Stable duplicate-key write error code used by the server/driver for unique index violations on insert.
    /// </summary>
    internal const int DuplicateKeyWriteErrorCode = 11000;

    /// <summary>
    /// Converts a fault after a read-style operation into a typed <c>Robotico.Result.Result&lt;TEntity&gt;</c> failure.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TId">Identifier type.</typeparam>
    /// <param name="ex">Exception thrown by the driver.</param>
    /// <param name="id">Entity id used for error context.</param>
    /// <returns>Failure result; never success.</returns>
    internal static Robotico.Result.Result<TEntity> MapAfterGetById<TEntity, TId>(MongoException ex, TId id)
        where TEntity : IEntity<TId>
        where TId : notnull
    {
        return Robotico.Result.Result.Error<TEntity>(new ExceptionError(ex));
    }

    /// <summary>
    /// Converts a fault after <c>InsertOne</c> into a <see cref="Robotico.Result.Result"/>.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TId">Identifier type.</typeparam>
    /// <param name="ex">Exception thrown by the driver.</param>
    /// <param name="entity">Entity used for error context.</param>
    /// <returns>Failure result; never success.</returns>
    internal static Robotico.Result.Result MapAfterAdd<TEntity, TId>(MongoException ex, TEntity entity)
        where TEntity : IEntity<TId>
        where TId : notnull
    {
        if (ex is MongoWriteException writeEx && writeEx.WriteError?.Code == DuplicateKeyWriteErrorCode)
        {
            return Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' already exists.", "DUPLICATE"));
        }

        return Robotico.Result.Result.Error(new ExceptionError(ex));
    }

    /// <summary>
    /// Converts a fault after <c>ReplaceOne</c> into a <see cref="Robotico.Result.Result"/>.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TId">Identifier type.</typeparam>
    /// <param name="ex">Exception thrown by the driver.</param>
    /// <param name="entity">Entity used for error context.</param>
    /// <returns>Failure result; never success.</returns>
    internal static Robotico.Result.Result MapAfterReplace<TEntity, TId>(MongoException ex, TEntity entity)
        where TEntity : IEntity<TId>
        where TId : notnull
    {
        return Robotico.Result.Result.Error(new ExceptionError(ex));
    }

    /// <summary>
    /// Converts a fault after <c>DeleteOne</c> into a <see cref="Robotico.Result.Result"/>.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TId">Identifier type.</typeparam>
    /// <param name="ex">Exception thrown by the driver.</param>
    /// <param name="entity">Entity used for error context.</param>
    /// <returns>Failure result; never success.</returns>
    internal static Robotico.Result.Result MapAfterDelete<TEntity, TId>(MongoException ex, TEntity entity)
        where TEntity : IEntity<TId>
        where TId : notnull
    {
        return Robotico.Result.Result.Error(new ExceptionError(ex));
    }
}
