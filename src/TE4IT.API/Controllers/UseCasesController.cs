using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.UseCases.Responses;
using Commands = TE4IT.Application.Features.UseCases.Commands;
using Queries = TE4IT.Application.Features.UseCases.Queries;

namespace TE4IT.API.Controllers;

/// <summary>
/// Kullanım senaryosu yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UseCasesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Modül kullanım senaryolarını sayfalı olarak listeler
    /// </summary>
    [HttpGet("modules/{moduleId:guid}")]
    [Authorize(Policy = "UseCaseRead")]
    [ProducesResponseType(typeof(PagedResult<UseCaseListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByModule(
        Guid moduleId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var query = new Queries.ListUseCases.ListUseCasesQuery(moduleId, page, pageSize, isActive, search);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Kullanım senaryosu ID'sine göre detaylı bilgi getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "UseCaseRead")]
    [ProducesResponseType(typeof(UseCaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new Queries.GetUseCaseById.GetUseCaseByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Yeni kullanım senaryosu oluşturur
    /// </summary>
    [HttpPost("modules/{moduleId:guid}")]
    [Authorize(Policy = "UseCaseCreate")]
    [ProducesResponseType(typeof(Commands.CreateUseCase.CreateUseCaseCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        Guid moduleId,
        [FromBody] CreateUseCaseRequest request,
        CancellationToken ct)
    {
        var command = new Commands.CreateUseCase.CreateUseCaseCommand(moduleId, request.Title, request.Description, request.ImportantNotes);
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Kullanım senaryosunu günceller
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "UseCaseUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUseCaseRequest request,
        CancellationToken ct)
    {
        var command = new Commands.UpdateUseCase.UpdateUseCaseCommand(
            id, request.Title, request.Description, request.ImportantNotes);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Kullanım senaryosu aktif/pasif durumunu değiştirir
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "UseCaseUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeUseCaseStatusRequest request,
        CancellationToken ct)
    {
        var command = new Commands.ChangeUseCaseStatus.ChangeUseCaseStatusCommand(id, request.IsActive);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Kullanım senaryosunu siler
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "UseCaseDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await mediator.Send(new Commands.DeleteUseCase.DeleteUseCaseCommand(id), ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}

/// <summary>
/// Kullanım senaryosu oluşturma request DTO
/// </summary>
public record CreateUseCaseRequest(string Title, string? Description, string? ImportantNotes);

/// <summary>
/// Kullanım senaryosu güncelleme request DTO
/// </summary>
public record UpdateUseCaseRequest(string Title, string? Description, string? ImportantNotes);

/// <summary>
/// Kullanım senaryosu durum değiştirme request DTO
/// </summary>
public record ChangeUseCaseStatusRequest(bool IsActive);
