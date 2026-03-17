# Robotico.Repository.MongoDb

`IRepository<TEntity, TId>` and `IUnitOfWork` backed by MongoDB.Driver using `IMongoCollection<TEntity>` and optional `IClientSessionHandle`.

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![GitHub Packages](https://img.shields.io/badge/GitHub%20Packages-Robotico.Repository.MongoDb-blue?logo=github)](https://github.com/robotico-dev/robotico-repository-mongodb-csharp/packages)

## Session and transaction

Use the same `IClientSessionHandle` for `MongoDbRepository` and `MongoDbUnitOfWork` so that repository operations and commit participate in one transaction. Start the session with `client.StartSession()` and `session.StartTransaction()` before using repositories; pass the session to both the repository constructor and the unit of work. Call `CommitAsync` to commit the transaction.

## License

See repository license file.
