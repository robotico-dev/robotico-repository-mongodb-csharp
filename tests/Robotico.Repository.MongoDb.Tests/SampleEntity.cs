using MongoDB.Bson.Serialization.Attributes;
using Robotico.Domain;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>
/// Sample entity for MongoDB repository integration tests.
/// </summary>
public sealed class SampleEntity : IEntity<Guid>
{
    /// <inheritdoc />
    [BsonId]
    public Guid Id { get; init; }
}
