using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

namespace Robotico.Repository.MongoDb.Tests;

/// <summary>
/// Provides an <see cref="IMongoClient"/>: either from <c>ROBOTICO_MONGO_CONNECTION</c> (e.g. CI service container)
/// or from a throwaway MongoDB instance via Testcontainers when Docker is available locally.
/// </summary>
public sealed class MongoDbFixture : IAsyncLifetime
{
    private const string ConnectionEnvironmentVariableName = "ROBOTICO_MONGO_CONNECTION";

    private readonly string? _externalConnectionString;
    private MongoDbContainer? _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbFixture"/> class.
    /// </summary>
    public MongoDbFixture()
    {
        _externalConnectionString = Environment.GetEnvironmentVariable(ConnectionEnvironmentVariableName);
    }

    /// <summary>
    /// Client connected after <see cref="InitializeAsync"/> completes.
    /// </summary>
    public IMongoClient Client { get; private set; } = null!;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        if (!string.IsNullOrWhiteSpace(_externalConnectionString))
        {
            MongoClientSettings settings = MongoClientSettings.FromConnectionString(_externalConnectionString);
            Client = new MongoClient(settings);
            return;
        }

        _container = new MongoDbBuilder().Build();
        await _container.StartAsync();
        MongoClientSettings containerSettings = MongoClientSettings.FromConnectionString(_container.GetConnectionString());
        Client = new MongoClient(containerSettings);
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}
