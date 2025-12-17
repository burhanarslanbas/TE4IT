using MongoDB.Driver;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Persistence.EducationManagement.Repositories.Enrollments;

public sealed class EnrollmentWriteRepository : IEnrollmentWriteRepository
{
    private readonly IMongoCollection<Enrollment> enrollments;

    public EnrollmentWriteRepository(IMongoDatabase database)
    {
        enrollments = database.GetCollection<Enrollment>("enrollments");
    }

    public Task AddAsync(Enrollment entity, CancellationToken cancellationToken = default)
        => enrollments.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public void Update(Enrollment entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.Eq(e => e.Id, entity.Id);
        enrollments.ReplaceOne(filter, entity, cancellationToken: cancellationToken);
    }

    public void Remove(Enrollment entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.Eq(e => e.Id, entity.Id);
        enrollments.DeleteOne(filter, cancellationToken);
    }
}

