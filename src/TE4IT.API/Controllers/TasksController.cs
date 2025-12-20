using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Tasks.Responses;
using TE4IT.Domain.Enums;
using Commands = TE4IT.Application.Features.Tasks.Commands;
using Queries = TE4IT.Application.Features.Tasks.Queries;

namespace TE4IT.API.Controllers;

/// <summary>
/// Görev yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TasksController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Kullanım senaryosu görevlerini sayfalı olarak listeler
    /// </summary>
    [HttpGet("usecases/{useCaseId:guid}")]
    [Authorize(Policy = "TaskRead")]
    [ProducesResponseType(typeof(PagedResult<TaskListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByUseCase(
        Guid useCaseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TaskState? state = null,
        [FromQuery] TaskType? type = null,
        [FromQuery] Guid? assigneeId = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var query = new Queries.ListTasks.ListTasksQuery(useCaseId, page, pageSize, state, type, assigneeId, dueDateFrom, dueDateTo, search);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Görev ID'sine göre detaylı bilgi getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "TaskRead")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new Queries.GetTaskById.GetTaskByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Yeni görev oluşturur
    /// </summary>
    [HttpPost("usecases/{useCaseId:guid}")]
    [Authorize(Policy = "TaskCreate")]
    [ProducesResponseType(typeof(Commands.CreateTask.CreateTaskCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        Guid useCaseId,
        [FromBody] CreateTaskRequest request,
        CancellationToken ct)
    {
        var command = new Commands.CreateTask.CreateTaskCommand(useCaseId, request.Title, request.TaskType, request.Description, request.ImportantNotes, request.DueDate, request.AssigneeId);
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Görevi günceller
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "TaskUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken ct)
    {
        var command = new Commands.UpdateTask.UpdateTaskCommand(
            id, request.Title, request.Description, request.ImportantNotes, request.DueDate);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Görev durumunu değiştirir
    /// </summary>
    /// <remarks>
    /// Geçerli state geçişleri:
    /// - NotStarted → InProgress (Start)
    /// - InProgress → Completed (Complete)
    /// - NotStarted/InProgress → Cancelled (Cancel)
    /// - InProgress/Cancelled → NotStarted (Revert)
    /// </remarks>
    [HttpPatch("{id:guid}/state")]
    [Authorize(Policy = "TaskStateChange")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeState(
        Guid id,
        [FromBody] ChangeTaskStateRequest request,
        CancellationToken ct)
    {
        var command = new Commands.ChangeTaskState.ChangeTaskStateCommand(id, request.NewState);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Görevi başlatır
    /// </summary>
    /// <remarks>
    /// Görevi başlatır. Sadece NotStarted durumundaki görevler başlatılabilir.
    /// Görev atanmış olmalıdır.
    /// </remarks>
    [HttpPost("{id:guid}/start")]
    [Authorize(Policy = "TaskStateChange")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        var command = new Commands.StartTask.StartTaskCommand(id);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Görevi tamamlar
    /// </summary>
    /// <remarks>
    /// Görevi tamamlar. Sadece InProgress durumundaki görevler tamamlanabilir.
    /// Bloklayan bağımlılıklar varsa görev tamamlanamaz.
    /// Tamamlanma notu/açıklaması opsiyoneldir.
    /// </remarks>
    [HttpPost("{id:guid}/complete")]
    [Authorize(Policy = "TaskStateChange")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(
        Guid id,
        [FromBody] CompleteTaskRequest? request = null,
        CancellationToken ct = default)
    {
        var command = new Commands.CompleteTask.CompleteTaskCommand(id, request?.CompletionNote);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Görevi iptal eder
    /// </summary>
    /// <remarks>
    /// Görevi iptal eder. Sadece NotStarted veya InProgress durumundaki görevler iptal edilebilir.
    /// Completed durumundaki görevler iptal edilemez.
    /// </remarks>
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Policy = "TaskStateChange")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var command = new Commands.CancelTask.CancelTaskCommand(id);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Görevden atamayı kaldırır
    /// </summary>
    /// <remarks>
    /// Görevden atamayı kaldırır. Sadece NotStarted veya Cancelled durumundaki görevlerden atama kaldırılabilir.
    /// InProgress durumundaki görevlerden atama kaldırılamaz.
    /// </remarks>
    [HttpPost("{id:guid}/unassign")]
    [Authorize(Policy = "TaskAssign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unassign(Guid id, CancellationToken ct)
    {
        var command = new Commands.UnassignTask.UnassignTaskCommand(id);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Görevi bir kişiye atar ve başlatır
    /// </summary>
    /// <remarks>
    /// Görevi belirtilen kişiye atar ve aynı anda başlatır.
    /// Görev NotStarted durumunda olmalıdır.
    /// </remarks>
    [HttpPost("{id:guid}/assign-and-start")]
    [Authorize(Policy = "TaskAssign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignAndStart(
        Guid id,
        [FromBody] AssignTaskRequest request,
        CancellationToken ct)
    {
        var command = new Commands.AssignAndStartTask.AssignAndStartTaskCommand(id, request.AssigneeId);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Görevi bir kişiye atar
    /// </summary>
    [HttpPost("{id:guid}/assign")]
    [Authorize(Policy = "TaskAssign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(
        Guid id,
        [FromBody] AssignTaskRequest request,
        CancellationToken ct)
    {
        var command = new Commands.AssignTask.AssignTaskCommand(id, request.AssigneeId);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Görevi siler
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "TaskDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await mediator.Send(new Commands.DeleteTask.DeleteTaskCommand(id), ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}

/// <summary>
/// Görev oluşturma request DTO
/// </summary>
public record CreateTaskRequest(string Title, TaskType TaskType, string? Description, string? ImportantNotes, DateTime? DueDate, Guid? AssigneeId = null);

/// <summary>
/// Görev güncelleme request DTO
/// </summary>
public record UpdateTaskRequest(string Title, string? Description, string? ImportantNotes, DateTime? DueDate);

/// <summary>
/// Görev durum değiştirme request DTO
/// </summary>
public record ChangeTaskStateRequest(TaskState NewState);

/// <summary>
/// Görev atama request DTO
/// </summary>
public record AssignTaskRequest(Guid AssigneeId);

/// <summary>
/// Görev tamamlama request DTO
/// </summary>
public record CompleteTaskRequest(string? CompletionNote);
