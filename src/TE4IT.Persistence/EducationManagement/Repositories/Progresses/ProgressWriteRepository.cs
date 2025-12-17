using MongoDB.Driver;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Persistence.EducationManagement.Repositories.Progresses;

public sealed class ProgressWriteRepository : IProgressWriteRepository
{
    private readonly IMongoCollection<Progress> progresses;

    public ProgressWriteRepository(IMongoDatabase database)
    {
        progresses = database.GetCollection<Progress>("progresses");
    }

    public Task AddAsync(Progress entity, CancellationToken cancellationToken = default)
        => progresses.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public void Update(Progress entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.Eq(p => p.Id, entity.Id);
        progresses.ReplaceOne(filter, entity, cancellationToken: cancellationToken);
    }

    public void Remove(Progress entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.Eq(p => p.Id, entity.Id);
        progresses.DeleteOne(filter, cancellationToken);
    }

    public Task UpsertAsync(Progress entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.And(
            Builders<Progress>.Filter.Eq(p => p.UserId, entity.UserId),
            Builders<Progress>.Filter.Eq(p => p.ContentId, entity.ContentId),
            Builders<Progress>.Filter.Eq(p => p.CourseId, entity.CourseId));

        var options = new ReplaceOptions { IsUpsert = true };
        return progresses.ReplaceOneAsync(filter, entity, options, cancellationToken);
    }
}

