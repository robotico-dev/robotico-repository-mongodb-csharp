# Implementation checklist (10/10 quality)

Quality bar: [ARCHITECT-RATINGS-CSHARP-IMPLEMENTATIONS.adoc](../../docs/ARCHITECT-RATINGS-CSHARP-IMPLEMENTATIONS.adoc). Align with **robotico-results-csharp** and **robotico-repository-inmemory-csharp**.

- [ ] Implement `MongoDbRepository<TEntity, TId>` : `IRepository<TEntity, TId>` using `IMongoDatabase`/collection; use `IClientSessionHandle` when part of a UoW.
- [ ] Implement `MongoDbUnitOfWork` : `IUnitOfWork`; `CommitAsync` → `session.CommitTransactionAsync()`.
- [ ] Collection name strategy (e.g. by entity type); map MongoDB exceptions to `Result.Error` (NOT_FOUND, DUPLICATE, etc.).
- [ ] XML docs, guards, cancellation on async APIs.
- [ ] Unit tests: success, not found, duplicate, null, cancellation.

Reference: `Robotico.Repository.InMemory` in robotico-repository-inmemory-csharp.
