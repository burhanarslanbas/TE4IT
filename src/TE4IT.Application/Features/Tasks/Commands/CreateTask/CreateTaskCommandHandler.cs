using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Enums;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandHandler(IUnitOfWork unitOfWork, ITaskWriteRepository taskRepository)
    : IRequestHandler<CreateTaskCommand, CreateTaskCommandResponse>
{
    public async System.Threading.Tasks.Task<CreateTaskCommandResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var creator = (UserId)request.CreatorId;
        var taskType = (TaskType)request.TaskType;

        var entity = TE4IT.Domain.Entities.Task.Create(
            useCaseId: request.UseCaseId,
            creatorId: creator,
            title: request.Title,
            taskType: taskType,
            description: request.Description,
            importantNotes: request.ImportantNotes
        );

        if (request.DueDate.HasValue)
        {
            entity.UpdateDueDate(request.DueDate);
        }

        await taskRepository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTaskCommandResponse { Id = entity.Id };
    }
}



