using MongoDB.Driver;
using Xunit;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>Repository operations with <see cref="IClientSessionHandle"/> (transactional paths).</summary>
[Collection("MongoDb")]
public sealed class MongoDbRepositorySessionTests
{
    private readonly MongoDbFixture _fixture;

    public MongoDbRepositorySessionTests(MongoDbFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetById_with_session_sees_insert_before_commit()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase("sess_read_" + Guid.NewGuid().ToString("N"));
        IMongoCollection<SampleEntity> collection = database.GetCollection<SampleEntity>("entities");
        IClientSessionHandle session = await _fixture.Client.StartSessionAsync();
        session.StartTransaction();
        MongoDbRepository<SampleEntity, Guid> repo = new(collection, session);
        SampleEntity entity = new() { Id = Guid.NewGuid() };

        Assert.True(repo.Add(entity).IsSuccess());
        Robotico.Result.Result<SampleEntity> get = repo.GetById(entity.Id);
        Assert.True(get.IsSuccess(out SampleEntity? loaded));
        Assert.Equal(entity.Id, loaded!.Id);
        await session.AbortTransactionAsync();
    }

    [Fact]
    public async Task Update_and_Remove_with_session_commit_persist()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase("sess_crud_" + Guid.NewGuid().ToString("N"));
        IMongoCollection<SampleEntity> collection = database.GetCollection<SampleEntity>("entities");
        IClientSessionHandle session = await _fixture.Client.StartSessionAsync();
        session.StartTransaction();
        MongoDbRepository<SampleEntity, Guid> repo = new(collection, session);
        MongoDbUnitOfWork uow = new(session);
        SampleEntity entity = new() { Id = Guid.NewGuid() };

        Assert.True(repo.Add(entity).IsSuccess());
        Assert.True(repo.Update(new SampleEntity { Id = entity.Id }).IsSuccess());
        Assert.True(repo.Remove(new SampleEntity { Id = entity.Id }).IsSuccess());
        Assert.True((await uow.CommitAsync()).IsSuccess());

        MongoDbRepository<SampleEntity, Guid> read = new(collection, null);
        Assert.True(read.GetById(entity.Id).IsError(out _));
    }
}
