using Xunit;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>
/// xUnit collection that shares one MongoDB container across test classes.
/// </summary>
[CollectionDefinition("MongoDb")]
public sealed class MongoDbTestCollection : ICollectionFixture<MongoDbFixture>
{
}
