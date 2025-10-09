using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Relational.Db;
using TE4IT.Persistence.Relational.Repositories.Base;

namespace TE4IT.Persistence.Relational.Repositories.UseCases;

public sealed class UseCaseReadRepository(AppDbContext db)
    : ReadRepository<UseCase>(db), IUseCaseReadRepository
{
}

