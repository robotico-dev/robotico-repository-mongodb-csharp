using MongoDB.Bson.Serialization.Attributes;
using Robotico.Domain;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>
/// Entity with string id for null-guard repository tests.
/// </summary>
public sealed class SampleEntityWithStringId : IEntity<string>
{
    /// <inheritdoc />
    [BsonId]
    public string Id { get; init; } = "";
}
