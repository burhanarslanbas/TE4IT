using MediatR;
using TE4IT.Domain.Enums.Education;

namespace TE4IT.Application.Features.Education.Roadmaps.Commands.CreateRoadmap;

public sealed record CreateRoadmapCommand(
    Guid CourseId,
    string Title,
    string? Description,
    int EstimatedDurationMinutes,
    IReadOnlyList<StepDto> Steps) : IRequest<CreateRoadmapCommandResponse>;

public sealed record StepDto(
    string Title,
    string? Description,
    int Order,
    bool IsRequired,
    int EstimatedDurationMinutes,
    IReadOnlyList<ContentDto> Contents);

public sealed record ContentDto(
    ContentType Type,
    string Title,
    string? Description,
    int Order,
    bool IsRequired,
    string? Content,
    string? LinkUrl);

