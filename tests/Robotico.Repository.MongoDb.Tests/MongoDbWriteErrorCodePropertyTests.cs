using CsCheck;
using Xunit;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>
/// Property-style sampling documenting the stable MongoDB duplicate-key write error code (driver contract).
/// </summary>
public sealed class MongoDbWriteErrorCodePropertyTests
{
    [Fact]
    public void Duplicate_key_error_code_is_stable_under_sampling()
    {
        Gen.Const(MongoDbRepositoryMongoExceptionRouter.DuplicateKeyWriteErrorCode).Sample(static code => code == 11000);
    }
}
