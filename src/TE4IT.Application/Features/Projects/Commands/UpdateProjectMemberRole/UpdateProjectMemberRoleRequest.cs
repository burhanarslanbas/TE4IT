using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Projects.Commands.UpdateProjectMemberRole;

/// <summary>
/// Proje üyesi rolü güncelleme request DTO
/// </summary>
public sealed record UpdateProjectMemberRoleRequest(ProjectRole Role);
