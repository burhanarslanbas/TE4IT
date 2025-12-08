using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Projects.Commands.AcceptProjectInvitation;
using TE4IT.Application.Features.Projects.Commands.CancelProjectInvitation;
using TE4IT.Application.Features.Projects.Commands.CreateProject;
using TE4IT.Application.Features.Projects.Commands.InviteProjectMember;
using TE4IT.Application.Features.Projects.Commands.RemoveProjectMember;
using TE4IT.Application.Features.Projects.Commands.UpdateProjectMemberRole;
using TE4IT.Application.Features.Projects.Queries.GetProjectById;
using TE4IT.Application.Features.Projects.Queries.GetProjectInvitationByToken;
using TE4IT.Application.Features.Projects.Queries.ListProjectInvitations;
using TE4IT.Application.Features.Projects.Queries.ListProjectMembers;
using TE4IT.Application.Features.Projects.Queries.ListProjects;
using TE4IT.Application.Features.Projects.Responses;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Services;

namespace TE4IT.API.Controllers;

/// <summary>
/// Proje yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProjectsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Projeleri sayfalı olarak listeler
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProjectListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var query = new ListProjectsQuery(page, pageSize, isActive, search);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Proje ID'sine göre detaylı bilgi getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ProjectRead")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetProjectByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Yeni proje oluşturur    
    /// </summary> 
    [HttpPost]
    // Role based authorization
    // [Authorize (Roles = $"{RoleNames.Administrator},{RoleNames.OrganizationManager},{RoleNames.TeamLead},{RoleNames.Trainer}")]
    // Policy based authorization
    [Authorize(Policy = "ProjectCreate")]
    [ProducesResponseType(typeof(CreateProjectCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateProjectCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Projeyi günceller
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken ct)
    {
        var command = new TE4IT.Application.Features.Projects.Commands.UpdateProject.UpdateProjectCommand(id, request.Title, request.Description);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Proje aktif/pasif durumunu değiştirir
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeProjectStatusRequest request, CancellationToken ct)
    {
        var command = new TE4IT.Application.Features.Projects.Commands.ChangeProjectStatus.ChangeProjectStatusCommand(id, request.IsActive);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Projeyi siler
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ProjectDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await mediator.Send(new TE4IT.Application.Features.Projects.Commands.DeleteProject.DeleteProjectCommand(id), ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Proje üyelerini listeler
    /// </summary>
    [HttpGet("{projectId:guid}/members")]
    [Authorize(Policy = "ProjectRead")]
    [ProducesResponseType(typeof(List<ProjectMemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListMembers(Guid projectId, CancellationToken ct)
    {
        var query = new ListProjectMembersQuery(projectId);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Projeden üye çıkarır
    /// </summary>
    [HttpDelete("{projectId:guid}/members/{userId:guid}")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMember(Guid projectId, Guid userId, CancellationToken ct)
    {
        var command = new RemoveProjectMemberCommand(projectId, userId);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Proje üyesinin rolünü günceller
    /// </summary>
    [HttpPut("{projectId:guid}/members/{userId:guid}/role")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMemberRole(Guid projectId, Guid userId, [FromBody] UpdateProjectMemberRoleRequest request, CancellationToken ct)
    {
        var command = new UpdateProjectMemberRoleCommand(projectId, userId, request.Role);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    // ========== PROJECT INVITATION ENDPOINTS ==========

    /// <summary>
    /// Projeye email ile davet gönderir
    /// </summary>
    [HttpPost("{projectId:guid}/invitations")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(typeof(InviteProjectMemberCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> InviteMember(Guid projectId, [FromBody] InviteProjectMemberRequest request, CancellationToken ct)
    {
        var command = new InviteProjectMemberCommand(projectId, request.Email, request.Role);
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetInvitationByToken), new { token = "placeholder" }, result);
    }

    /// <summary>
    /// Token ile davet bilgisini getirir (Public endpoint)
    /// </summary>
    [HttpGet("invitations/{token}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProjectInvitationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvitationByToken(string token, CancellationToken ct)
    {
        var query = new GetProjectInvitationByTokenQuery(token);
        var result = await mediator.Send(query, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Daveti kabul eder
    /// </summary>
    [HttpPost("invitations/{token}/accept")]
    [Authorize]
    [ProducesResponseType(typeof(AcceptProjectInvitationCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AcceptInvitation(string token, CancellationToken ct)
    {
        var command = new AcceptProjectInvitationCommand(token);
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Daveti iptal eder
    /// </summary>
    [HttpDelete("{projectId:guid}/invitations/{invitationId:guid}")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelInvitation(Guid projectId, Guid invitationId, CancellationToken ct)
    {
        var command = new CancelProjectInvitationCommand(projectId, invitationId);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Proje davetlerini listeler
    /// </summary>
    [HttpGet("{projectId:guid}/invitations")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(typeof(List<ProjectInvitationListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListInvitations(Guid projectId, [FromQuery] string? status = null, CancellationToken ct = default)
    {
        var query = new ListProjectInvitationsQuery(projectId, status);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }
}

/// <summary>
/// Proje güncelleme request DTO
/// </summary>
public record UpdateProjectRequest(string Title, string? Description);

/// <summary>
/// Proje durum değiştirme request DTO
/// </summary>
public record ChangeProjectStatusRequest(bool IsActive);

/// <summary>
/// Proje üyesi rolü güncelleme request DTO
/// </summary>
public record UpdateProjectMemberRoleRequest(ProjectRole Role);

/// <summary>
/// Proje daveti gönderme request DTO
/// </summary>
public record InviteProjectMemberRequest(string Email, ProjectRole Role);