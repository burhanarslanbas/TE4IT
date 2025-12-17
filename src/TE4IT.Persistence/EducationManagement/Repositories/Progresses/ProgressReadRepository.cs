using MongoDB.Driver;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;
using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Persistence.EducationManagement.Repositories.Progresses;

public sealed class ProgressReadRepository : IProgressReadRepository
{
    private readonly IMongoCollection<Progress> progresses;

    public ProgressReadRepository(IMongoDatabase database)
    {
        progresses = database.GetCollection<Progress>("progresses");
    }

    public Task<List<Progress>> GetAllAsync(CancellationToken cancellationToken = default)
        => progresses.Find(FilterDefinition<Progress>.Empty).ToListAsync(cancellationToken);

    public async Task<Progress?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.Eq(p => p.Id, id);
        return await progresses.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.Eq(p => p.Id, id);
        var count = await progresses.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    public async Task<PagedResult<Progress>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var filter = FilterDefinition<Progress>.Empty;
        var total = (int)await progresses.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await progresses.Find(filter)
            .SortByDescending(p => p.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Progress>(items, total, page, pageSize);
    }

    public async Task<Progress?> GetByUserAndContentAsync(Guid userId, Guid contentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.And(
            Builders<Progress>.Filter.Eq(p => p.UserId, userId),
            Builders<Progress>.Filter.Eq(p => p.ContentId, contentId),
            Builders<Progress>.Filter.Eq(p => p.CourseId, courseId));

        return await progresses.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Progress>> GetUserProgressByCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.And(
            Builders<Progress>.Filter.Eq(p => p.UserId, userId),
            Builders<Progress>.Filter.Eq(p => p.CourseId, courseId));

        return await progresses.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<int> GetCompletedContentCountAsync(Guid userId, Guid stepId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.And(
            Builders<Progress>.Filter.Eq(p => p.UserId, userId),
            Builders<Progress>.Filter.Eq(p => p.StepId, stepId),
            Builders<Progress>.Filter.Eq(p => p.CourseId, courseId),
            Builders<Progress>.Filter.Eq(p => p.IsCompleted, true));

        var count = await progresses.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return (int)count;
    }
}

