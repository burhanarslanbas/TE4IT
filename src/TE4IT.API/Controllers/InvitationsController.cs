using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Commands = TE4IT.Application.Features.Invitations.Commands;
using Queries = TE4IT.Application.Features.Invitations.Queries;

namespace TE4IT.API.Controllers;

/// <summary>
/// Proje daveti yönetimi endpoint'leri (Token-based operations)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public sealed class InvitationsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Token ile davet bilgisini getirir (Public endpoint)
    /// </summary>
    [HttpGet("{token}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Queries.GetInvitationByToken.InvitationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByToken(string token, CancellationToken ct)
    {
        // URL encode edilmiş token'ı decode et (örn: %3D -> =)
        var decodedToken = WebUtility.UrlDecode(token);
        var query = new Queries.GetInvitationByToken.GetInvitationByTokenQuery(decodedToken);
        var result = await mediator.Send(query, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Daveti kabul eder
    /// </summary>
    [HttpPost("{token}/accept")]
    [Authorize]
    [ProducesResponseType(typeof(Commands.AcceptInvitation.AcceptInvitationCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Accept(string token, CancellationToken ct)
    {
        // URL encode edilmiş token'ı decode et (örn: %3D -> =)
        var decodedToken = WebUtility.UrlDecode(token);
        var command = new Commands.AcceptInvitation.AcceptInvitationCommand(decodedToken);
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcının davetlerini listeler. Status parametresi boş bırakılırsa tüm davetler, belirtilirse ilgili durumdaki davetler döner.
    /// </summary>
    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(List<Queries.ListMyInvitations.MyInvitationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListMyInvitations([FromQuery] string? status = null, CancellationToken ct = default)
    {
        var query = new Queries.ListMyInvitations.ListMyInvitationsQuery(status);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Projeye email ile davet gönderir (Proje sahibi işlemi)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(typeof(Commands.SendInvitation.SendInvitationCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SendInvitation([FromBody] Commands.SendInvitation.SendInvitationRequest request, CancellationToken ct)
    {
        var command = new Commands.SendInvitation.SendInvitationCommand(request.ProjectId, request.Email, request.Role);
        var result = await mediator.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Daveti iptal eder
    /// </summary>
    [HttpDelete("{invitationId:guid}")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelInvitation(Guid invitationId, [FromBody] Commands.CancelInvitation.CancelInvitationRequest request, CancellationToken ct)
    {
        var command = new Commands.CancelInvitation.CancelInvitationCommand(request.ProjectId, invitationId);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Proje davetlerini listeler
    /// </summary>
    [HttpGet("projects/{projectId:guid}")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(typeof(List<Queries.ListInvitations.InvitationListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListInvitations(Guid projectId, [FromQuery] string? status = null, CancellationToken ct = default)
    {
        var query = new Queries.ListInvitations.ListInvitationsQuery(projectId, status);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }
}
