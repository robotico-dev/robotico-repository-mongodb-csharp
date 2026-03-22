using CsCheck;
using MongoDB.Driver;
using Robotico.Result.Errors;
using Xunit;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>
/// Property-based checks for <see cref="MongoDbRepositoryMongoExceptionRouter"/> mapping.
/// </summary>
public sealed class MongoDbRepositoryMongoExceptionRouterPropertyTests
{
    [Fact]
    public void MapAfterGetById_any_MongoException_yields_ExceptionError()
    {
        Gen.Guid.Sample(static id =>
        {
            MongoException ex = new MongoException("test");
            Robotico.Result.Result<SampleEntity> result =
                MongoDbRepositoryMongoExceptionRouter.MapAfterGetById<SampleEntity, Guid>(ex, id);
            return result.IsError(out IError? err) && err is ExceptionError;
        });
    }

    [Fact]
    public void MapAfterAdd_plain_MongoException_yields_ExceptionError()
    {
        Gen.String.Sample(static message =>
        {
            SampleEntity entity = new() { Id = Guid.NewGuid() };
            MongoException ex = new MongoException(message);
            Robotico.Result.Result result = MongoDbRepositoryMongoExceptionRouter.MapAfterAdd<SampleEntity, Guid>(ex, entity);
            return result.IsError(out IError? err) && err is ExceptionError;
        });
    }
}
