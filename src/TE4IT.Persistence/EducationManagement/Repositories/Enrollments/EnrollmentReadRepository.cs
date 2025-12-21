using MongoDB.Driver;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Persistence.EducationManagement.Repositories.Enrollments;

public sealed class EnrollmentReadRepository : IEnrollmentReadRepository
{
    private readonly IMongoCollection<Enrollment> enrollments;

    public EnrollmentReadRepository(IMongoDatabase database)
    {
        enrollments = database.GetCollection<Enrollment>("enrollments");
    }

    public Task<List<Enrollment>> GetAllAsync(CancellationToken cancellationToken = default)
        => enrollments.Find(FilterDefinition<Enrollment>.Empty).ToListAsync(cancellationToken);

    public Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.Eq(e => e.Id, id);
        return enrollments.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.Eq(e => e.Id, id);
        var count = await enrollments.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    public async Task<PagedResult<Enrollment>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var filter = FilterDefinition<Enrollment>.Empty;
        var total = (int)await enrollments.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await enrollments.Find(filter)
            .SortByDescending(e => e.EnrolledAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Enrollment>(items, total, page, pageSize);
    }

    public Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.And(
            Builders<Enrollment>.Filter.Eq(e => e.UserId, userId),
            Builders<Enrollment>.Filter.Eq(e => e.CourseId, courseId));

        return enrollments.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Enrollment>> GetUserEnrollmentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.Eq(e => e.UserId, userId);
        return await enrollments.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<bool> IsEnrolledAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var existing = await GetByUserAndCourseAsync(userId, courseId, cancellationToken);
        return existing is not null && existing.IsActive;
    }

    public async Task<int> GetEnrollmentCountByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.Eq(e => e.CourseId, courseId);
        var count = await enrollments.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return (int)count;
    }
}

