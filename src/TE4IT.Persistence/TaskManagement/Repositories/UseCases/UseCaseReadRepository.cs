using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.UseCases;

public sealed class UseCaseReadRepository(AppDbContext db)
    : ReadRepository<UseCase>(db), IUseCaseReadRepository
{
}

