using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Auth.Commands.Users.AssignRoleToUser;
using TE4IT.Application.Features.Auth.Commands.Users.RemoveRoleFromUser;
using TE4IT.Application.Features.Auth.Queries.Users.GetAllUsers;
using TE4IT.Application.Features.Auth.Queries.Users.GetUserById;
using TE4IT.Application.Features.Auth.Queries.Users.GetUserRoles;

namespace TE4IT.API.Controllers;

/// <summary>
/// Kullanıcı yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController(IMediator mediator) : ControllerBase
{
     /// <summary>
    /// Tüm kullanıcıları sayfalı olarak listeler
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Administrator")]
    
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var query = new GetAllUsersQuery();
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcı ID'sine göre getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetUserByIdQuery(id);
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
        var query = new GetUserRolesQuery(id);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcıya rol atar (Sadece Administrator)
    /// </summary>
    [HttpPost("{id:guid}/roles/{roleName}")]
    //[Authorize(Roles = "Administrator")]
    [ProducesResponseType(typeof(AssignRoleToUserCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole(
        Guid id,
        string roleName,
        CancellationToken ct)
    {
        var command = new AssignRoleToUserCommand(id, roleName);
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcıdan rol kaldırır (Sadece Administrator)
    /// </summary>
    [HttpDelete("{id:guid}/roles/{roleName}")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveRole(
        Guid id,
        string roleName,
        CancellationToken ct)
    {
        var command = new RemoveRoleFromUserCommand(id, roleName);
        await mediator.Send(command, ct);
        return NoContent();
    }
}
