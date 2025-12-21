using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Features.Education.Enrollments.Commands.EnrollInCourse;
using TE4IT.Application.Features.Education.Enrollments.Queries.GetUserEnrollments;

namespace TE4IT.API.Controllers;

/// <summary>
/// Kurs kayıt (enrollment) yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/education")]
[Authorize]
public sealed class EnrollmentsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Kullanıcıyı belirtilen kursa kaydeder
    /// </summary>
    /// <param name="courseId">Kurs ID'si</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Kayıt bilgileri</returns>
    [HttpPost("courses/{courseId:guid}/enroll")]
    [ProducesResponseType(typeof(EnrollInCourseCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnrollInCourse(Guid courseId, CancellationToken ct)
    {
        var command = new EnrollInCourseCommand(courseId);
        var result = await mediator.Send(command, ct);
        
        return CreatedAtAction(
            nameof(GetUserEnrollments), 
            null, 
            result);
    }

    /// <summary>
    /// Kullanıcının kurs kayıtlarını listeler
    /// </summary>
    /// <param name="status">Kayıt durumu filtresi (active/completed/all)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Kayıt listesi</returns>
    [HttpGet("enrollments")]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserEnrollments(
        [FromQuery] string? status = "all",
        CancellationToken ct = default)
    {
        var query = new GetUserEnrollmentsQuery(status);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }
}
