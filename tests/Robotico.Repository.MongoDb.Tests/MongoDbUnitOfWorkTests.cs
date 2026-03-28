using MongoDB.Driver;
using Robotico.Result.Errors;
using Xunit;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>
/// Integration tests for <see cref="MongoDbUnitOfWork"/>.
/// </summary>
[Collection("MongoDb")]
public sealed class MongoDbUnitOfWorkTests
{
    private readonly MongoDbFixture _fixture;

    public MongoDbUnitOfWorkTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CommitAsync_commits_transaction_with_repository_writes()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase("uow_tests_" + Guid.NewGuid().ToString("N"));
        IMongoCollection<SampleEntity> collection = database.GetCollection<SampleEntity>("entities");
        IClientSessionHandle session = await _fixture.Client.StartSessionAsync();
        session.StartTransaction();
        MongoDbRepository<SampleEntity, Guid> repo = new(collection, session);
        MongoDbUnitOfWork uow = new(session);
        SampleEntity entity = new() { Id = Guid.NewGuid() };

        Robotico.Result.Result addResult = repo.Add(entity);
        Assert.True(addResult.IsSuccess());

        Robotico.Result.Result commitResult = await uow.CommitAsync();
        Assert.True(commitResult.IsSuccess());

        MongoDbRepository<SampleEntity, Guid> readRepo = new(collection, null);
        Robotico.Result.Result<SampleEntity> getResult = readRepo.GetById(entity.Id);
        Assert.True(getResult.IsSuccess());
    }

    [Fact]
    public async Task CommitAsync_throws_OperationCanceledException_when_canceled()
    {
        IClientSessionHandle session = await _fixture.Client.StartSessionAsync();
        session.StartTransaction();
        MongoDbUnitOfWork uow = new(session);
        using CancellationTokenSource cts = new();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() => uow.CommitAsync(cts.Token));
    }

    [Fact]
    public async Task CommitAsync_returns_ExceptionError_when_transaction_was_aborted()
    {
        IClientSessionHandle session = await _fixture.Client.StartSessionAsync();
        session.StartTransaction();
        await session.AbortTransactionAsync();
        MongoDbUnitOfWork uow = new(session);

        Robotico.Result.Result r = await uow.CommitAsync();

        Assert.True(r.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }
}
