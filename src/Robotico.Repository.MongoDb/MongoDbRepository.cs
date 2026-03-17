using MongoDB.Driver;
using Robotico.Domain;
using Robotico.Repository;
using Robotico.Result;
using Robotico.Result.Errors;

namespace Robotico.Repository.MongoDb;

/// <summary>
/// MongoDB implementation of <see cref="IRepository{TEntity, TId}"/> using <see cref="IMongoCollection{TEntity}"/>.
/// </summary>
/// <remarks>
/// <para>When using with a transaction, pass the same <see cref="IClientSessionHandle"/> to both <see cref="MongoDbRepository{TEntity, TId}"/> and <see cref="MongoDbUnitOfWork"/> so that operations and commit participate in one transaction.</para>
/// <para><typeparamref name="TEntity"/> must have a property <c>Id</c> of type <typeparamref name="TId"/> (e.g. mapped to <c>_id</c> by the driver).</para>
/// </remarks>
/// <typeparam name="TEntity">The entity type (must implement <see cref="IEntity{TId}"/>).</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public sealed class MongoDbRepository<TEntity, TId>(IMongoCollection<TEntity> collection, IClientSessionHandle? session = null) : IRepository<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : notnull
{
    private static readonly FilterDefinitionBuilder<TEntity> F = Builders<TEntity>.Filter;

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null.</exception>
    public Robotico.Result.Result<TEntity> GetById(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        try
        {
            FilterDefinition<TEntity> filter = F.Eq(x => x.Id, id);
            TEntity? entity = session is null
                ? collection.Find(filter).FirstOrDefault()
                : collection.Find(session, filter).FirstOrDefault();
            return entity is null
                ? Robotico.Result.Result.Error<TEntity>(new SimpleError($"Entity with id '{id}' not found.", "NOT_FOUND"))
                : Robotico.Result.Result.Success(entity);
        }
        catch (MongoException ex)
        {
            return Robotico.Result.Result.Error<TEntity>(new Robotico.Result.Errors.ExceptionError(ex));
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    public Robotico.Result.Result Add(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        try
        {
            if (session is null)
            {
                collection.InsertOne(entity);
            }
            else
            {
                collection.InsertOne(session, entity);
            }

            return Robotico.Result.Result.Success();
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Code == 11000)
        {
            return Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' already exists.", "DUPLICATE"));
        }
        catch (MongoException ex)
        {
            return Robotico.Result.Result.Error(new Robotico.Result.Errors.ExceptionError(ex));
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    public Robotico.Result.Result Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        try
        {
            FilterDefinition<TEntity> filter = F.Eq(x => x.Id, entity.Id);
            ReplaceOneResult result = session is null
                ? collection.ReplaceOne(filter, entity)
                : collection.ReplaceOne(session, filter, entity);
            return result.MatchedCount > 0
                ? Robotico.Result.Result.Success()
                : Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' not found.", "NOT_FOUND"));
        }
        catch (MongoException ex)
        {
            return Robotico.Result.Result.Error(new Robotico.Result.Errors.ExceptionError(ex));
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    public Robotico.Result.Result Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        try
        {
            FilterDefinition<TEntity> filter = F.Eq(x => x.Id, entity.Id);
            DeleteResult result = session is null
                ? collection.DeleteOne(filter)
                : collection.DeleteOne(session, filter);
            return result.DeletedCount > 0
                ? Robotico.Result.Result.Success()
                : Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' not found.", "NOT_FOUND"));
        }
        catch (MongoException ex)
        {
            return Robotico.Result.Result.Error(new Robotico.Result.Errors.ExceptionError(ex));
        }
    }
}
