using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Features.Education.Roadmaps.Commands.CreateRoadmap;
using TE4IT.Application.Features.Education.Roadmaps.Commands.UpdateRoadmap;
using TE4IT.Application.Features.Education.Roadmaps.Queries.GetRoadmapByCourseId;
using TE4IT.Application.Features.Education.Roadmaps.Responses;

namespace TE4IT.API.Controllers;

/// <summary>
/// Roadmap yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/education")]
[Authorize]
public sealed class RoadmapsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Kurs ID'sine göre roadmap bilgisini getirir
    /// </summary>
    /// <param name="courseId">Kurs ID'si</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Roadmap detayları</returns>
    [HttpGet("courses/{courseId:guid}/roadmap")]
    [Authorize(Policy = "RoadmapView")]
    [ProducesResponseType(typeof(RoadmapResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCourseId(Guid courseId, CancellationToken ct)
    {
        var query = new GetRoadmapByCourseIdQuery(courseId);
        var result = await mediator.Send(query, ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Kurs ID'sine göre roadmap adımlarını getirir
    /// </summary>
    /// <param name="courseId">Kurs ID'si</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Roadmap adımları</returns>
    [HttpGet("courses/{courseId:guid}/roadmap/steps")]
    [Authorize(Policy = "RoadmapView")]
    [ProducesResponseType(typeof(IReadOnlyList<StepResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStepsByCourseId(Guid courseId, CancellationToken ct)
    {
        var query = new GetRoadmapByCourseIdQuery(courseId);
        var result = await mediator.Send(query, ct);
        if (result is null) return NotFound();
        return Ok(result.Steps);
    }

    /// <summary>
    /// Kurs için yeni roadmap oluşturur
    /// </summary>
    /// <param name="courseId">Kurs ID'si</param>
    /// <param name="command">Roadmap oluşturma bilgileri</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Oluşturulan roadmap bilgisi</returns>
    [HttpPost("courses/{courseId:guid}/roadmap")]
    [Authorize(Policy = "RoadmapCreate")]
    [ProducesResponseType(typeof(CreateRoadmapCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        Guid courseId,
        [FromBody] CreateRoadmapCommand command,
        CancellationToken ct)
    {
        var createCommand = command with { CourseId = courseId };
        var result = await mediator.Send(createCommand, ct);
        
        return CreatedAtAction(
            nameof(GetByCourseId), 
            new { courseId }, 
            result);
    }

    /// <summary>
    /// Kurs için roadmap'i günceller
    /// </summary>
    /// <param name="courseId">Kurs ID'si</param>
    /// <param name="command">Roadmap güncelleme bilgileri</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Güncelleme sonucu</returns>
    [HttpPut("courses/{courseId:guid}/roadmap")]
    [Authorize(Policy = "RoadmapUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid courseId,
        [FromBody] UpdateRoadmapCommand command,
        CancellationToken ct)
    {
        var updateCommand = command with { CourseId = courseId };
        var ok = await mediator.Send(updateCommand, ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}
