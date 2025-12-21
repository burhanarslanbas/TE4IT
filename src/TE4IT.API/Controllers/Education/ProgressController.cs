using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.API.Requests.Education;
using TE4IT.Application.Features.Education.Progresses.Commands.CompleteContent;
using TE4IT.Application.Features.Education.Progresses.Commands.UpdateVideoProgress;
using TE4IT.Application.Features.Education.Progresses.Queries.GetCourseProgress;
using TE4IT.Application.Features.Education.Progresses.Queries.GetProgressDashboard;

namespace TE4IT.API.Controllers;

/// <summary>
/// İlerleme takibi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/education")]
[Authorize]
public sealed class ProgressController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Belirtilen kurs için kullanıcının ilerleme bilgisini getirir
    /// </summary>
    /// <param name="courseId">Kurs ID'si</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Detaylı ilerleme bilgisi</returns>
    [HttpGet("courses/{courseId:guid}/progress")]
    [Authorize(Policy = "ProgressView")]
    [ProducesResponseType(typeof(CourseProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseProgress(Guid courseId, CancellationToken ct)
    {
        var query = new GetCourseProgressQuery(courseId);
        var result = await mediator.Send(query, ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcının tüm kurslar için ilerleme özet bilgisini getirir (dashboard)
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>İlerleme dashboard'u</returns>
    [HttpGet("progress/dashboard")]
    [Authorize(Policy = "ProgressView")]
    [ProducesResponseType(typeof(ProgressDashboardResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProgressDashboard(CancellationToken ct)
    {
        var query = new GetProgressDashboardQuery();
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// İçeriği tamamlandı olarak işaretler
    /// </summary>
    /// <param name="contentId">İçerik ID'si</param>
    /// <param name="request">Tamamlanma bilgileri</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Tamamlanma durumu</returns>
    [HttpPost("contents/{contentId:guid}/complete")]
    [Authorize(Policy = "ProgressUpdate")]
    [ProducesResponseType(typeof(CompleteContentCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteContent(
        Guid contentId,
        [FromBody] CompleteContentRequest request,
        CancellationToken ct)
    {
        var command = new CompleteContentCommand(
            contentId,
            request.CourseId,
            request.TimeSpentMinutes,
            request.WatchedPercentage);
        
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Video içeriği için ilerleme kaydeder (opsiyonel, real-time tracking)
    /// </summary>
    /// <param name="contentId">Video içerik ID'si</param>
    /// <param name="request">Video ilerleme bilgileri</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>İlerleme durumu</returns>
    [HttpPost("contents/{contentId:guid}/video-progress")]
    [Authorize(Policy = "ProgressUpdate")]
    [ProducesResponseType(typeof(UpdateVideoProgressCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVideoProgress(
        Guid contentId,
        [FromBody] UpdateVideoProgressRequest request,
        CancellationToken ct)
    {
        var command = new UpdateVideoProgressCommand(
            contentId,
            request.CourseId,
            request.WatchedPercentage,
            request.TimeSpentSeconds,
            request.IsCompleted);
        
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }
}
