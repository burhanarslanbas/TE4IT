using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Common.Pagination;
using Commands = TE4IT.Application.Features.Projects.Commands;
using Queries = TE4IT.Application.Features.Projects.Queries;
using TE4IT.Application.Features.Projects.Responses;

namespace TE4IT.API.Controllers;

/// <summary>
/// Proje yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public sealed class ProjectsController(IMediator mediator) : ControllerBase
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
        var query = new Queries.ListProjects.ListProjectsQuery(page, pageSize, isActive, search);
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
        var query = new Queries.GetProjectById.GetProjectByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Yeni proje oluşturur    
    /// </summary> 
    [HttpPost]
    [Authorize(Policy = "ProjectCreate")]
    [ProducesResponseType(typeof(Commands.CreateProject.CreateProjectCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] Commands.CreateProject.CreateProjectCommand command, CancellationToken ct)
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
    public async Task<IActionResult> Update(Guid id, [FromBody] Commands.UpdateProject.UpdateProjectRequest request, CancellationToken ct)
    {
        var command = new Commands.UpdateProject.UpdateProjectCommand(id, request.Title, request.Description);
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
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] Commands.ChangeProjectStatus.ChangeProjectStatusRequest request, CancellationToken ct)
    {
        var command = new Commands.ChangeProjectStatus.ChangeProjectStatusCommand(id, request.IsActive);
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
        var command = new Commands.DeleteProject.DeleteProjectCommand(id);
        var ok = await mediator.Send(command, ct);
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
        var query = new Queries.ListProjectMembers.ListProjectMembersQuery(projectId);
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
        var command = new Commands.RemoveProjectMember.RemoveProjectMemberCommand(projectId, userId);
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
    public async Task<IActionResult> UpdateMemberRole(Guid projectId, Guid userId, [FromBody] Commands.UpdateProjectMemberRole.UpdateProjectMemberRoleRequest request, CancellationToken ct)
    {
        var command = new Commands.UpdateProjectMemberRole.UpdateProjectMemberRoleCommand(projectId, userId, request.Role);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}
