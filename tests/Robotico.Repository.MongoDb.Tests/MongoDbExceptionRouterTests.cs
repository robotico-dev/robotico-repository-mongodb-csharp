using MongoDB.Driver;
using Robotico.Result.Errors;
using Xunit;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>Unit tests for <see cref="MongoDbRepositoryMongoExceptionRouter"/> (no container).</summary>
public sealed class MongoDbExceptionRouterTests
{
    [Fact]
    public void MapAfterGetById_maps_to_ExceptionError()
    {
        MongoException ex = new MongoClientException("read failed");
        Robotico.Result.Result<SampleEntity> r = MongoDbRepositoryMongoExceptionRouter.MapAfterGetById<SampleEntity, Guid>(ex, Guid.Empty);
        Assert.True(r.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }

    [Fact]
    public void MapAfterAdd_maps_non_duplicate_write_to_ExceptionError()
    {
        MongoException ex = new MongoClientException("insert failed");
        Robotico.Result.Result r = MongoDbRepositoryMongoExceptionRouter.MapAfterAdd<SampleEntity, Guid>(ex, new SampleEntity { Id = Guid.NewGuid() });
        Assert.True(r.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }

    [Fact]
    public void MapAfterReplace_maps_to_ExceptionError()
    {
        MongoException ex = new MongoClientException("replace failed");
        Robotico.Result.Result r = MongoDbRepositoryMongoExceptionRouter.MapAfterReplace<SampleEntity, Guid>(ex, new SampleEntity { Id = Guid.NewGuid() });
        Assert.True(r.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }

    [Fact]
    public void MapAfterDelete_maps_to_ExceptionError()
    {
        MongoException ex = new MongoClientException("delete failed");
        Robotico.Result.Result r = MongoDbRepositoryMongoExceptionRouter.MapAfterDelete<SampleEntity, Guid>(ex, new SampleEntity { Id = Guid.NewGuid() });
        Assert.True(r.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }
}
