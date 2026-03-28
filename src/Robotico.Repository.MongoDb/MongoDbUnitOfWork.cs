using MongoDB.Driver;
using Robotico.Result.Errors;

namespace Robotico.Repository.MongoDb;

/// <summary>
/// MongoDB <see cref="IUnitOfWork"/> that commits a <see cref="IClientSessionHandle"/> transaction.
/// </summary>
/// <remarks>
/// <para>Start the session with <c>client.StartSession()</c> and <c>session.StartTransaction()</c> before using repositories. Pass the same session to <see cref="MongoDbRepository{TEntity, TId}"/> and to this UoW. Call <see cref="IUnitOfWork.CommitAsync"/> to commit the transaction.</para>
/// </remarks>
public sealed class MongoDbUnitOfWork(IClientSessionHandle session) : IUnitOfWork
{
    /// <inheritdoc />
    public async Task<Robotico.Result.Result> CommitAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            return Robotico.Result.Result.Success();
        }
        catch (MongoException ex)
        {
            return Robotico.Result.Result.Error(new ExceptionError(ex));
        }
        catch (InvalidOperationException ex)
        {
            return Robotico.Result.Result.Error(new ExceptionError(ex));
        }
    }
}
