using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Education.Courses.Requests;
using TE4IT.Application.Features.Education.Courses.Responses;
using Commands = TE4IT.Application.Features.Education.Courses.Commands;
using Queries = TE4IT.Application.Features.Education.Courses.Queries;

namespace TE4IT.API.Controllers;

/// <summary>
/// Kurs yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/education/courses")]
[Authorize]
public sealed class CoursesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Kursları sayfalı olarak listeler
    /// </summary>
    /// <param name="page">Sayfa numarası (varsayılan: 1)</param>
    /// <param name="pageSize">Sayfa başına kayıt sayısı (varsayılan: 10)</param>
    /// <param name="search">Arama terimi (title veya description)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Sayfalı kurs listesi</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CourseListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var query = new Queries.GetCourses.GetCoursesQuery(page, pageSize, search);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Kurs ID'sine göre detaylı bilgi getirir
    /// </summary>
    /// <param name="id">Kurs ID'si</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Kurs detayları</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "CourseView")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new Queries.GetCourseById.GetCourseByIdQuery(id);
        var result = await mediator.Send(query, ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Yeni kurs oluşturur
    /// </summary>
    /// <param name="command">Kurs oluşturma bilgileri</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Oluşturulan kurs bilgisi</returns>
    [HttpPost]
    [Authorize(Policy = "CourseCreate")]
    [ProducesResponseType(typeof(Commands.CreateCourse.CreateCourseCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] Commands.CreateCourse.CreateCourseCommand command, 
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Kursu günceller
    /// </summary>
    /// <param name="id">Kurs ID'si</param>
    /// <param name="request">Güncelleme bilgileri</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Güncelleme sonucu</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CourseUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken ct)
    {
        var command = new Commands.UpdateCourse.UpdateCourseCommand(
            id, 
            request.Title, 
            request.Description, 
            request.ThumbnailUrl);
        
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Kursu siler (soft delete)
    /// </summary>
    /// <param name="id">Kurs ID'si</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Silme sonucu</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CourseDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new Commands.DeleteCourse.DeleteCourseCommand(id);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}