namespace TE4IT.Application.Features.Projects.Commands.ChangeProjectStatus;

/// <summary>
/// Proje durum değiştirme request DTO
/// </summary>
public sealed record ChangeProjectStatusRequest(bool IsActive);
