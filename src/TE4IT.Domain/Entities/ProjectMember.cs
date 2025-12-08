using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Events;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Domain.Entities;

/// <summary>
/// Proje üyeliği entity'si - kullanıcıların projelere erişim yetkilerini yönetir
/// </summary>
public class ProjectMember : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public UserId UserId { get; private set; } = null!;
    public ProjectRole Role { get; private set; }
    public DateTime JoinedDate { get; private set; }

    private ProjectMember() { }

    /// <summary>
    /// Proje üyesi oluşturur
    /// </summary>
    /// <param name="projectId">Proje ID'si</param>
    /// <param name="userId">Kullanıcı ID'si</param>
    /// <param name="role">Proje rolü (Owner, Member, Viewer)</param>
    /// <returns>Oluşturulan ProjectMember</returns>
    public static ProjectMember Create(Guid projectId, UserId userId, ProjectRole role)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));

        if (userId == null)
            throw new ArgumentNullException(nameof(userId));

        // Owner rolü sadece proje oluşturulurken atanabilir, burada kontrol edilmez
        // Çünkü CreateProjectCommandHandler'da proje sahibi otomatik Owner olarak eklenir
        if (role != ProjectRole.Owner && role != ProjectRole.Member && role != ProjectRole.Viewer)
            throw new ArgumentException("Role must be Owner, Member, or Viewer.", nameof(role));

        var projectMember = new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = role,
            JoinedDate = DateTime.UtcNow
        };

        projectMember.AddDomainEvent(new ProjectMemberAddedEvent(
            projectId,
            userId.Value,
            (int)role,
            projectMember.JoinedDate));

        return projectMember;
    }

    /// <summary>
    /// Proje üyesinin rolünü günceller
    /// </summary>
    /// <param name="newRole">Yeni rol (Member veya Viewer)</param>
    /// <exception cref="InvalidOperationException">Owner rolü değiştirilemez</exception>
    public void UpdateRole(ProjectRole newRole)
    {
        if (Role == ProjectRole.Owner)
            throw new InvalidOperationException("Owner rolü değiştirilemez.");

        if (newRole == ProjectRole.Owner)
            throw new ArgumentException("Owner rolü atanamaz.", nameof(newRole));

        if (newRole != ProjectRole.Member && newRole != ProjectRole.Viewer)
            throw new ArgumentException("Role must be Member or Viewer.", nameof(newRole));

        if (Role == newRole)
            return;

        var oldRole = Role;
        Role = newRole;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new ProjectMemberRoleChangedEvent(
            ProjectId,
            UserId.Value,
            (int)oldRole,
            (int)newRole));
    }

    /// <summary>
    /// Proje üyesini siler (domain event ekler)
    /// </summary>
    public void Remove()
    {
        AddDomainEvent(new ProjectMemberRemovedEvent(ProjectId, UserId.Value));
    }
}

