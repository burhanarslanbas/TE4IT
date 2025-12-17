using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Common.Pagination;
using Commands = TE4IT.Application.Features.Modules.Commands;
using Queries = TE4IT.Application.Features.Modules.Queries;
using TE4IT.Application.Features.Modules.Responses;

namespace TE4IT.API.Controllers;

/// <summary>
/// Modül yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ModulesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Proje modüllerini sayfalı olarak listeler
    /// </summary>
    [HttpGet("projects/{projectId:guid}")]
    [Authorize(Policy = "ModuleRead")]
    [ProducesResponseType(typeof(PagedResult<ModuleListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var query = new Queries.ListModules.ListModulesQuery(projectId, page, pageSize, isActive, search);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Modül ID'sine göre detaylı bilgi getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ModuleRead")]
    [ProducesResponseType(typeof(ModuleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new Queries.GetModuleById.GetModuleByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Yeni modül oluşturur
    /// </summary>
    [HttpPost("projects/{projectId:guid}")]
    [Authorize(Policy = "ModuleCreate")]
    [ProducesResponseType(typeof(Commands.CreateModule.CreateModuleCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreateModuleRequest request,
        CancellationToken ct)
    {
        var command = new Commands.CreateModule.CreateModuleCommand(projectId, request.Title, request.Description);
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Modülü günceller
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ModuleUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateModuleRequest request,
        CancellationToken ct)
    {
        var command = new Commands.UpdateModule.UpdateModuleCommand(id, request.Title, request.Description);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Modül aktif/pasif durumunu değiştirir
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "ModuleUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeModuleStatusRequest request,
        CancellationToken ct)
    {
        var command = new Commands.ChangeModuleStatus.ChangeModuleStatusCommand(id, request.IsActive);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Modülü siler
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ModuleDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await mediator.Send(new Commands.DeleteModule.DeleteModuleCommand(id), ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}

/// <summary>
/// Modül oluşturma request DTO
/// </summary>
public record CreateModuleRequest(string Title, string? Description);

/// <summary>
/// Modül güncelleme request DTO
/// </summary>
public record UpdateModuleRequest(string Title, string? Description);

/// <summary>
/// Modül durum değiştirme request DTO
/// </summary>
public record ChangeModuleStatusRequest(bool IsActive);
