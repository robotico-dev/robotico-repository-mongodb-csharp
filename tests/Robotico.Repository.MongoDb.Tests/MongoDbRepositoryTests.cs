using MongoDB.Driver;
using Robotico.Result.Errors;
using Xunit;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>
/// Integration tests for <see cref="MongoDbRepository{TEntity,TId}"/>.
/// </summary>
[Collection("MongoDb")]
public sealed class MongoDbRepositoryTests
{
    private readonly MongoDbFixture _fixture;

    public MongoDbRepositoryTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    private static string NewDatabaseName()
    {
        return "repo_tests_" + Guid.NewGuid().ToString("N");
    }

    [Fact]
    public void Add_then_GetById_returns_same_entity()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase(NewDatabaseName());
        IMongoCollection<SampleEntity> collection = database.GetCollection<SampleEntity>("entities");
        MongoDbRepository<SampleEntity, Guid> repo = new(collection, null);
        SampleEntity entity = new() { Id = Guid.NewGuid() };

        Robotico.Result.Result addResult = repo.Add(entity);
        Assert.True(addResult.IsSuccess());

        Robotico.Result.Result<SampleEntity> getResult = repo.GetById(entity.Id);
        Assert.True(getResult.IsSuccess(out SampleEntity? loaded));
        Assert.Equal(entity.Id, loaded!.Id);
    }

    [Fact]
    public void GetById_returns_NOT_FOUND_when_missing()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase(NewDatabaseName());
        IMongoCollection<SampleEntity> collection = database.GetCollection<SampleEntity>("entities");
        MongoDbRepository<SampleEntity, Guid> repo = new(collection, null);

        Robotico.Result.Result<SampleEntity> result = repo.GetById(Guid.NewGuid());

        Assert.True(result.IsError(out IError? err));
        Assert.Equal("NOT_FOUND", err!.Code);
    }

    [Fact]
    public void Add_returns_DUPLICATE_when_id_exists()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase(NewDatabaseName());
        IMongoCollection<SampleEntity> collection = database.GetCollection<SampleEntity>("entities");
        MongoDbRepository<SampleEntity, Guid> repo = new(collection, null);
        Guid id = Guid.NewGuid();
        repo.Add(new SampleEntity { Id = id });

        Robotico.Result.Result result = repo.Add(new SampleEntity { Id = id });

        Assert.True(result.IsError(out IError? err));
        Assert.Equal("DUPLICATE", err!.Code);
    }

    [Fact]
    public void Update_returns_NOT_FOUND_when_missing()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase(NewDatabaseName());
        IMongoCollection<SampleEntity> collection = database.GetCollection<SampleEntity>("entities");
        MongoDbRepository<SampleEntity, Guid> repo = new(collection, null);
        SampleEntity entity = new() { Id = Guid.NewGuid() };

        Robotico.Result.Result result = repo.Update(entity);

        Assert.True(result.IsError(out IError? err));
        Assert.Equal("NOT_FOUND", err!.Code);
    }

    [Fact]
    public void Remove_returns_NOT_FOUND_when_missing()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase(NewDatabaseName());
        IMongoCollection<SampleEntity> collection = database.GetCollection<SampleEntity>("entities");
        MongoDbRepository<SampleEntity, Guid> repo = new(collection, null);
        SampleEntity entity = new() { Id = Guid.NewGuid() };

        Robotico.Result.Result result = repo.Remove(entity);

        Assert.True(result.IsError(out IError? err));
        Assert.Equal("NOT_FOUND", err!.Code);
    }

    [Fact]
    public void GetById_throws_ArgumentNullException_when_id_is_null()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase(NewDatabaseName());
        IMongoCollection<SampleEntityWithStringId> collection = database.GetCollection<SampleEntityWithStringId>("entities");
        MongoDbRepository<SampleEntityWithStringId, string> repo = new(collection, null);

        Assert.Throws<ArgumentNullException>(() => repo.GetById(null!));
    }

    [Fact]
    public void Add_throws_ArgumentNullException_when_entity_is_null()
    {
        IMongoDatabase database = _fixture.Client.GetDatabase(NewDatabaseName());
        IMongoCollection<SampleEntity> collection = database.GetCollection<SampleEntity>("entities");
        MongoDbRepository<SampleEntity, Guid> repo = new(collection, null);

        Assert.Throws<ArgumentNullException>(() => repo.Add(null!));
    }
}
