using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Common.Pagination;
using Commands = TE4IT.Application.Features.Auth.Commands.Users;
using Queries = TE4IT.Application.Features.Auth.Queries.Users;

namespace TE4IT.API.Controllers;

/// <summary>
/// Kullanıcı yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController(IMediator mediator) : ControllerBase
{
     /// <summary>
    /// Tüm kullanıcıları sayfalı olarak listeler
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Queries.GetUserById.UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var query = new Queries.GetAllUsers.GetAllUsersQuery();
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcı ID'sine göre getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Queries.GetUserById.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new Queries.GetUserById.GetUserByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Kullanıcının rollerini getirir
    /// </summary>
    [HttpGet("{id:guid}/roles")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRoles(Guid id, CancellationToken ct)
    {
        var query = new Queries.GetUserRoles.GetUserRolesQuery(id);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcıya rol atar (Sadece Administrator)
    /// </summary>
    [HttpPost("{id:guid}/roles/{roleName}")]
    [ProducesResponseType(typeof(Commands.AssignRoleToUser.AssignRoleToUserCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole(
        Guid id,
        string roleName,
        CancellationToken ct)
    {
        var command = new Commands.AssignRoleToUser.AssignRoleToUserCommand(id, roleName);
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcıdan rol kaldırır (Sadece Administrator)
    /// </summary>
    [HttpDelete("{id:guid}/roles/{roleName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveRole(
        Guid id,
        string roleName,
        CancellationToken ct)
    {
        var command = new Commands.RemoveRoleFromUser.RemoveRoleFromUserCommand(id, roleName);
        await mediator.Send(command, ct);
        return NoContent();
    }
}
