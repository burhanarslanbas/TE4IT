using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Abstractions.Persistence.Repositories.TaskRelations;
using TaskEntity = TE4IT.Domain.Entities.Task;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.TaskRelations.Commands.CreateTaskRelation;

public sealed class CreateTaskRelationCommandHandler(
    ITaskReadRepository taskReadRepository,
    ITaskWriteRepository taskWriteRepository,
    ITaskRelationWriteRepository taskRelationWriteRepository,
    IUseCaseReadRepository useCaseReadRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<CreateTaskRelationCommand, bool>
{
    public async Task<bool> Handle(CreateTaskRelationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        if (request.SourceTaskId == request.TargetTaskId)
            throw new BusinessRuleViolationException("Görev kendisiyle ilişkilendirilemez.");

        var sourceTask = await taskReadRepository.GetByIdAsync(request.SourceTaskId, cancellationToken);
        if (sourceTask is null)
            throw new ResourceNotFoundException("Kaynak görev bulunamadı.");

        // Proje bilgisini al (UseCase -> Module -> Project)
        var useCase = await useCaseReadRepository.GetByIdAsync(sourceTask.UseCaseId, cancellationToken);
        if (useCase is null)
            throw new ResourceNotFoundException("Kullanım senaryosu bulunamadı.");

        var module = await moduleReadRepository.GetByIdAsync(useCase.ModuleId, cancellationToken);
        if (module is null)
            throw new ResourceNotFoundException("Modül bulunamadı.");

        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Proje bulunamadı.");

        // Erişim kontrolü: Kullanıcının kaynak görevi düzenleme yetkisi var mı?
        if (!userPermissionService.CanEditTask(currentUserId, sourceTask, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Görev ilişkisi oluşturma yetkiniz bulunmamaktadır.");

        var targetTask = await taskReadRepository.GetByIdAsync(request.TargetTaskId, cancellationToken);
        if (targetTask is null)
            throw new ResourceNotFoundException("Hedef görev bulunamadı.");

        // Mevcut relation'ı kontrol et (unique constraint)
        var existingRelation = sourceTask.Relations
            .FirstOrDefault(r => 
                r.SourceTaskId == request.SourceTaskId && 
                r.TargetTaskId == request.TargetTaskId && 
                r.RelationType == request.RelationType);
        
        if (existingRelation != null)
            throw new BusinessRuleViolationException("Bu görev ilişkisi zaten mevcut.");

        var relation = new TaskRelation
        {
            SourceTaskId = request.SourceTaskId,
            TargetTaskId = request.TargetTaskId,
            RelationType = request.RelationType
        };

        // Domain method'u çağır (validation için)
        sourceTask.AddRelation(relation);
        
        // TaskRelation'ı doğrudan DbContext'e ekle (In-Memory database tracking sorunlarını önlemek için)
        await taskRelationWriteRepository.AddAsync(relation, cancellationToken);
        
        // Task'ı update et (UpdatedDate güncellenmesi için)
        taskWriteRepository.Update(sourceTask, cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

