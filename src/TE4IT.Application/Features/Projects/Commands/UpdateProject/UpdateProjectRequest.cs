namespace TE4IT.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Proje güncelleme request DTO
/// </summary>
public sealed record UpdateProjectRequest(string Title, string? Description);
