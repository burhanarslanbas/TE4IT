using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Projects.Commands.CreateProject;
using TE4IT.Application.Features.Projects.Queries.GetProjectById;
using TE4IT.Application.Features.Projects.Queries.ListProjects;
using TE4IT.Application.Features.Projects.Responses;
using TE4IT.Domain.Constants;

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
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var query = new ListProjectsQuery(page, pageSize);
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
    public async Task<IActionResult> Update(Guid id, [FromBody] TE4IT.Application.Features.Projects.Commands.UpdateProject.UpdateProjectCommand command, CancellationToken ct)
    {
        if (id != command.ProjectId) return BadRequest("Mismatched id");
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
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] TE4IT.Application.Features.Projects.Commands.ChangeProjectStatus.ChangeProjectStatusCommand command, CancellationToken ct)
    {
        if (id != command.ProjectId) return BadRequest("Mismatched id");
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
}