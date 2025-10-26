using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Features.Auth.Commands.Roles.CreateRole;
using TE4IT.Application.Features.Auth.Commands.Roles.DeleteRole;
using TE4IT.Application.Features.Auth.Commands.Roles.UpdateRole;
using TE4IT.Application.Features.Auth.Queries.Roles.GetAllRoles;
using TE4IT.Application.Features.Auth.Queries.Roles.GetRoleById;

namespace TE4IT.API.Controllers;

/// <summary>
/// Rol yönetimi endpoint'leri (Sadece Administrator erişebilir)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RolesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Tüm rolleri listeler
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var query = new GetAllRolesQuery();
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Rol ID'sine göre getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetRoleByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Yeni rol oluşturur
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateRoleCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.RoleId }, result);
    }

    /// <summary>
    /// Rolü günceller
    /// </summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(UpdateRoleCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken ct)
    {
        var command = new UpdateRoleCommand(id, request.RoleName);
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Rolü siler
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new DeleteRoleCommand(id);
        await mediator.Send(command, ct);
        return NoContent();
    }
}

/// <summary>
/// Rol güncelleme request DTO
/// </summary>
public record UpdateRoleRequest(string RoleName);
