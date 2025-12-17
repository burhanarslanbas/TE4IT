using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;
using TaskEntity = TE4IT.Domain.Entities.Task;

namespace TE4IT.Infrastructure.Auth.Services.Authorization;

/// <summary>
/// Kullanıcı yetki servisi implementasyonu - proje ve görev erişim yetkilerini kontrol eder
/// </summary>
public sealed class UserPermissionService : IUserPermissionService
{
    private readonly IProjectMemberReadRepository _projectMemberRepository;
    private readonly IUserInfoService _userInfoService;

    public UserPermissionService(
        IProjectMemberReadRepository projectMemberRepository,
        IUserInfoService userInfoService)
    {
        _projectMemberRepository = projectMemberRepository;
        _userInfoService = userInfoService;
    }

    public bool CanAccessProject(UserId userId, Project project)
    {
        if (project == null)
            return false;

        // Sistem yöneticisi her projeye erişebilir
        if (IsSystemAdministrator(userId))
            return true;

        // Proje oluşturan kişi erişebilir
        if (project.CreatorId == userId)
            return true;

        // Proje üyesi mi kontrol et
        var member = _projectMemberRepository.GetByProjectIdAndUserIdAsync(project.Id, userId.Value).GetAwaiter().GetResult();
        return member != null;
    }

    public bool CanEditProject(UserId userId, Project project)
    {
        if (project == null)
            return false;

        // Sistem yöneticisi düzenleyebilir
        if (IsSystemAdministrator(userId))
            return true;

        var role = GetUserProjectRole(userId, project);
        // Owner veya Member düzenleyebilir (Viewer düzenleyemez)
        return role.HasValue && (role.Value == ProjectRole.Owner || role.Value == ProjectRole.Member);
    }

    public bool CanDeleteProject(UserId userId, Project project)
    {
        if (project == null)
            return false;

        // Sistem yöneticisi silebilir
        if (IsSystemAdministrator(userId))
            return true;

        // Sadece Owner silebilir
        var role = GetUserProjectRole(userId, project);
        return role.HasValue && role.Value == ProjectRole.Owner;
    }

    public bool CanViewTask(UserId userId, TaskEntity task, Project project)
    {
        if (task == null || project == null)
            return false;

        // Sistem yöneticisi görebilir
        if (IsSystemAdministrator(userId))
            return true;

        // Proje sahibi (Owner) tüm task'ları görebilir
        var role = GetUserProjectRole(userId, project);
        if (role.HasValue && role.Value == ProjectRole.Owner)
            return true;

        // Proje üyesi (Member veya Viewer) görebilir
        if (role.HasValue)
            return true;

        // Task'ı oluşturan veya atanan kişi görebilir
        return task.CreatorId == userId || (task.AssigneeId != null && task.AssigneeId == userId);
    }

    public bool CanEditTask(UserId userId, TaskEntity task, Project project)
    {
        if (task == null || project == null)
            return false;

        // Sistem yöneticisi düzenleyebilir
        if (IsSystemAdministrator(userId))
            return true;

        // Proje sahibi (Owner) tüm task'ları düzenleyebilir
        var role = GetUserProjectRole(userId, project);
        if (role.HasValue && role.Value == ProjectRole.Owner)
            return true;

        // Proje üyesi (Member) düzenleyebilir (Viewer düzenleyemez)
        if (role.HasValue && role.Value == ProjectRole.Member)
            return true;

        // Task'ı oluşturan veya atanan kişi düzenleyebilir
        return task.CreatorId == userId || (task.AssigneeId != null && task.AssigneeId == userId);
    }

    public bool CanDeleteTask(UserId userId, TaskEntity task, Project project)
    {
        if (task == null || project == null)
            return false;

        // Sistem yöneticisi silebilir
        if (IsSystemAdministrator(userId))
            return true;

        // Proje sahibi (Owner) tüm task'ları silebilir
        var role = GetUserProjectRole(userId, project);
        if (role.HasValue && role.Value == ProjectRole.Owner)
            return true;

        // Proje üyesi (Member) silebilir (Viewer silemez)
        if (role.HasValue && role.Value == ProjectRole.Member)
            return true;

        // Task'ı oluşturan silebilir
        return task.CreatorId == userId;
    }

    public bool CanAssignTask(UserId userId, TaskEntity task, Project project)
    {
        if (task == null || project == null)
            return false;

        // Sistem yöneticisi atayabilir
        if (IsSystemAdministrator(userId))
            return true;

        // Proje sahibi (Owner) tüm task'ları atayabilir
        var role = GetUserProjectRole(userId, project);
        if (role.HasValue && role.Value == ProjectRole.Owner)
            return true;

        // Proje üyesi (Member) atayabilir (Viewer atayamaz)
        if (role.HasValue && role.Value == ProjectRole.Member)
            return true;

        // Task'ı oluşturan atayabilir
        return task.CreatorId == userId;
    }

    public bool CanCreateModule(UserId userId, Project project)
    {
        if (project == null)
            return false;

        // Sistem yöneticisi oluşturabilir
        if (IsSystemAdministrator(userId))
            return true;

        var role = GetUserProjectRole(userId, project);
        // Owner veya Member oluşturabilir (Viewer oluşturamaz)
        return role.HasValue && (role.Value == ProjectRole.Owner || role.Value == ProjectRole.Member);
    }

    public bool CanCreateUseCase(UserId userId, Module module, Project project)
    {
        if (module == null || project == null)
            return false;

        // Sistem yöneticisi oluşturabilir
        if (IsSystemAdministrator(userId))
            return true;

        // Proje rolüne göre kontrol: Owner veya Member oluşturabilir (Viewer oluşturamaz)
        var role = GetUserProjectRole(userId, project);
        return role.HasValue && (role.Value == ProjectRole.Owner || role.Value == ProjectRole.Member);
    }

    public bool CanCreateTask(UserId userId, UseCase useCase, Project project)
    {
        if (useCase == null || project == null)
            return false;

        // Sistem yöneticisi oluşturabilir
        if (IsSystemAdministrator(userId))
            return true;

        // Proje rolüne göre kontrol: Owner veya Member oluşturabilir (Viewer oluşturamaz)
        var role = GetUserProjectRole(userId, project);
        return role.HasValue && (role.Value == ProjectRole.Owner || role.Value == ProjectRole.Member);
    }

    public ProjectRole? GetUserProjectRole(UserId userId, Project project)
    {
        if (project == null)
            return null;

        // Sistem yöneticisi için özel rol yok, IsSystemAdministrator ile kontrol edilir
        if (IsSystemAdministrator(userId))
            return null;

        // Proje oluşturan kişi Owner
        if (project.CreatorId == userId)
            return ProjectRole.Owner;

        // ProjectMembers tablosundan rolü al
        var member = _projectMemberRepository.GetByProjectIdAndUserIdAsync(project.Id, userId.Value).GetAwaiter().GetResult();
        return member?.Role;
    }

    public bool IsSystemAdministrator(UserId userId)
    {
        var userInfo = _userInfoService.GetUserInfoAsync(userId.Value, CancellationToken.None).GetAwaiter().GetResult();
        if (userInfo == null)
            return false;

        return userInfo.Roles.Contains(RoleNames.Administrator, StringComparer.OrdinalIgnoreCase);
    }
}

