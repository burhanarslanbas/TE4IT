using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Features.TaskRelations.Commands.CreateTaskRelation;
using TE4IT.Application.Features.TaskRelations.Commands.DeleteTaskRelation;
using TE4IT.Application.Features.TaskRelations.Queries.GetTaskRelations;
using TE4IT.Application.Features.Tasks.Responses;
using TE4IT.Domain.Enums;

namespace TE4IT.API.Controllers;

/// <summary>
/// Görev ilişkisi yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/tasks/{taskId:guid}/relations")]
[Authorize]
public class TaskRelationsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Görev ilişkilerini getirir
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "TaskRelationRead")]
    [ProducesResponseType(typeof(List<TaskRelationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRelations(Guid taskId, CancellationToken ct)
    {
        var query = new GetTaskRelationsQuery(taskId);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Yeni görev ilişkisi oluşturur
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "TaskRelationCreate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        Guid taskId,
        [FromBody] CreateTaskRelationRequest request,
        CancellationToken ct)
    {
        var command = new CreateTaskRelationCommand(taskId, request.TargetTaskId, request.RelationType);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Görev ilişkisini siler
    /// </summary>
    [HttpDelete("{relationId:guid}")]
    [Authorize(Policy = "TaskRelationDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid taskId, Guid relationId, CancellationToken ct)
    {
        var ok = await mediator.Send(new DeleteTaskRelationCommand(taskId, relationId), ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}

/// <summary>
/// Görev ilişkisi oluşturma request DTO
/// </summary>
public record CreateTaskRelationRequest(Guid TargetTaskId, TaskRelationType RelationType);

