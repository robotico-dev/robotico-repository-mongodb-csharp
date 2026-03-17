# Robotico.Repository.MongoDb

`IRepository<TEntity, TId>` and `IUnitOfWork` backed by MongoDB.Driver using `IMongoCollection<TEntity>` and optional `IClientSessionHandle`.

## Session and transaction

Use the same `IClientSessionHandle` for `MongoDbRepository` and `MongoDbUnitOfWork` so that repository operations and commit participate in one transaction. Start the session with `client.StartSession()` and `session.StartTransaction()` before using repositories; pass the session to both the repository constructor and the unit of work. Call `CommitAsync` to commit the transaction.

## License

See repository license file.
