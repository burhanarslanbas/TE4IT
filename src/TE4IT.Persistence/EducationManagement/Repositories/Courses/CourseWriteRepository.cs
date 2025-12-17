using MongoDB.Driver;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Persistence.EducationManagement.Repositories.Courses;

public sealed class CourseWriteRepository : ICourseWriteRepository
{
    private readonly IMongoCollection<Course> courses;

    public CourseWriteRepository(IMongoDatabase database)
    {
        courses = database.GetCollection<Course>("courses");
    }

    public Task AddAsync(Course entity, CancellationToken cancellationToken = default)
        => courses.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public void Update(Course entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        var filter = Builders<Course>.Filter.Eq(c => c.Id, entity.Id);
        courses.ReplaceOne(filter, entity, cancellationToken: cancellationToken);
    }

    public void Remove(Course entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Course>.Filter.Eq(c => c.Id, entity.Id);
        courses.DeleteOne(filter, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Course>.Filter.Eq(c => c.Id, id);
        var update = Builders<Course>.Update
            .Set(c => c.IsActive, false)
            .Set(c => c.UpdatedDate, DateTime.UtcNow);

        var result = await courses.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}

