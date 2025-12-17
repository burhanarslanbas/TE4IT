using MongoDB.Bson;
using MongoDB.Driver;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Persistence.EducationManagement.Repositories.Courses;

public sealed class CourseReadRepository : ICourseReadRepository
{
    private readonly IMongoCollection<Course> courses;

    public CourseReadRepository(IMongoDatabase database)
    {
        courses = database.GetCollection<Course>("courses");
    }

    public Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return courses.Find(FilterDefinition<Course>.Empty).ToListAsync(cancellationToken);
    }

    public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Course>.Filter.Eq(c => c.Id, id);
        return courses.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Course>.Filter.Eq(c => c.Id, id);
        var count = await courses.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    public async Task<PagedResult<Course>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var filter = FilterDefinition<Course>.Empty;
        var total = (int)await courses.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await courses.Find(filter)
            .SortByDescending(c => c.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Course>(items, total, page, pageSize);
    }

    public async Task<PagedResult<Course>> GetActiveAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var fb = Builders<Course>.Filter;
        var filter = fb.Eq(c => c.IsActive, true);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var regex = new BsonRegularExpression(search, "i");
            filter = fb.And(filter,
                fb.Or(
                    fb.Regex(c => c.Title, regex),
                    fb.Regex(c => c.Description, regex)));
        }

        var sort = Builders<Course>.Sort.Descending(c => c.CreatedDate);
        var total = (int)await courses.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await courses.Find(filter)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Course>(items, total, page, pageSize);
    }
}

