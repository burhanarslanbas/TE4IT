# Backend Task Management - Implementasyon Dokümanı

## 📋 Genel Bakış

Bu doküman, **Module**, **UseCase** ve **Task** yönetimi için backend implementasyon planını içerir. Project feature'ı referans alınarak aynı pattern ve yapı kullanılacaktır.

## 🏗️ Mimari Yapı

### Mevcut Durum
- ✅ Domain entities (Module, UseCase, Task, TaskRelation) mevcut
- ✅ Persistence repositories (Read/Write) mevcut
- ✅ EF Core configurations mevcut
- ⏳ Application layer (Commands, Queries, Handlers, Validators) yapılacak
- ⏳ API Controllers yapılacak
- ⏳ Authorization policies yapılacak

### Referans: Project Feature Yapısı
```
TE4IT.Application/Features/Projects/
├── Commands/
│   ├── CreateProject/
│   │   ├── CreateProjectCommand.cs
│   │   ├── CreateProjectCommandHandler.cs
│   │   ├── CreateProjectCommandValidator.cs
│   │   └── CreateProjectCommandResponse.cs
│   ├── UpdateProject/
│   ├── DeleteProject/
│   └── ChangeProjectStatus/
├── Queries/
│   ├── GetProjectById/
│   └── ListProjects/
└── Responses/
    ├── ProjectResponse.cs
    └── ProjectListItemResponse.cs
```

---

## 📦 1. Module Feature Implementation

### 1.1 Commands

#### CreateModuleCommand
**Dosya**: `src/TE4IT.Application/Features/Modules/Commands/CreateModule/CreateModuleCommand.cs`

```csharp
public sealed record CreateModuleCommand(
    Guid ProjectId,
    string Title,
    string? Description) : IRequest<CreateModuleCommandResponse>;
```

**Handler**: `CreateModuleCommandHandler`
- `ICurrentUser` ile creator ID alınır
- `IProjectReadRepository` ile projenin aktif olduğu kontrol edilir
- `Module.Create()` ile entity oluşturulur
- `IModuleWriteRepository.AddAsync()` ile kaydedilir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

**Validator**: `CreateModuleCommandValidator`
- `ProjectId`: NotEmpty
- `Title`: NotEmpty, MinLength(3), MaxLength(100)
- `Description`: MaxLength(1000) (opsiyonel)

**Response**: `CreateModuleCommandResponse`
```csharp
public sealed record CreateModuleCommandResponse
{
    public Guid Id { get; init; }
}
```

#### UpdateModuleCommand
**Dosya**: `src/TE4IT.Application/Features/Modules/Commands/UpdateModule/UpdateModuleCommand.cs`

```csharp
public sealed record UpdateModuleCommand(
    Guid ModuleId,
    string Title,
    string? Description) : IRequest<bool>;
```

**Handler**: `UpdateModuleCommandHandler`
- `IModuleReadRepository.GetByIdAsync()` ile module bulunur
- Module null ise `false` döner
- Projenin aktif olduğu kontrol edilir
- `Module.UpdateTitle()` ve `Module.UpdateDescription()` çağrılır
- `IModuleWriteRepository.Update()` ile güncellenir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

**Validator**: `UpdateModuleCommandValidator`
- `ModuleId`: NotEmpty
- `Title`: NotEmpty, MinLength(3), MaxLength(100)
- `Description`: MaxLength(1000) (opsiyonel)

#### ChangeModuleStatusCommand
**Dosya**: `src/TE4IT.Application/Features/Modules/Commands/ChangeModuleStatus/ChangeModuleStatusCommand.cs`

```csharp
public sealed record ChangeModuleStatusCommand(
    Guid ModuleId,
    bool IsActive) : IRequest<bool>;
```

**Handler**: `ChangeModuleStatusCommandHandler`
- `IModuleReadRepository.GetByIdAsync()` ile module bulunur
- Module null ise `false` döner
- `IsActive` true ise `Module.Activate()`, false ise `Module.Archive()` çağrılır
- `IModuleWriteRepository.Update()` ile güncellenir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

**Validator**: `ChangeModuleStatusCommandValidator`
- `ModuleId`: NotEmpty

#### DeleteModuleCommand
**Dosya**: `src/TE4IT.Application/Features/Modules/Commands/DeleteModule/DeleteModuleCommand.cs`

```csharp
public sealed record DeleteModuleCommand(Guid ModuleId) : IRequest<bool>;
```

**Handler**: `DeleteModuleCommandHandler`
- `IModuleReadRepository.GetByIdAsync()` ile module bulunur
- Module null ise `false` döner
- Altında use case var mı kontrol edilir (opsiyonel, RESTRICT için)
- `IModuleWriteRepository.Remove()` ile silinir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

**Validator**: `DeleteModuleCommandValidator`
- `ModuleId`: NotEmpty

### 1.2 Queries

#### GetModuleByIdQuery
**Dosya**: `src/TE4IT.Application/Features/Modules/Queries/GetModuleById/GetModuleByIdQuery.cs`

```csharp
public sealed record GetModuleByIdQuery(Guid ModuleId) : IRequest<ModuleResponse>;
```

**Handler**: `GetModuleByIdQueryHandler`
- `IModuleReadRepository.GetByIdAsync()` ile module bulunur
- Module null ise `KeyNotFoundException` fırlatılır
- `ModuleResponse` mapping yapılır

**Validator**: `GetModuleByIdQueryValidator`
- `ModuleId`: NotEmpty

#### ListModulesQuery
**Dosya**: `src/TE4IT.Application/Features/Modules/Queries/ListModules/ListModulesQuery.cs`

```csharp
public sealed record ListModulesQuery(
    Guid ProjectId,
    int Page = 1,
    int PageSize = 20,
    bool? IsActive = null,
    string? Search = null) : IRequest<PagedResult<ModuleListItemResponse>>;
```

**Handler**: `ListModulesQueryHandler`
- `IModuleReadRepository` üzerinden query yapılır
- `ProjectId` ile filtreleme
- `IsActive` ile filtreleme (opsiyonel)
- `Search` ile title'a göre arama (opsiyonel)
- Pagination uygulanır
- `ModuleListItemResponse` mapping yapılır

**Validator**: `ListModulesQueryValidator`
- `ProjectId`: NotEmpty
- `Page`: GreaterThan(0)
- `PageSize`: GreaterThan(0), LessThanOrEqualTo(100)

### 1.3 Responses

**Dosya**: `src/TE4IT.Application/Features/Modules/Responses/ModuleResponse.cs`
```csharp
public sealed class ModuleResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
}
```

**Dosya**: `src/TE4IT.Application/Features/Modules/Responses/ModuleListItemResponse.cs`
```csharp
public sealed class ModuleListItemResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
    public int UseCaseCount { get; init; } // Aggregation
}
```

### 1.4 API Controller

**Dosya**: `src/TE4IT.API/Controllers/ModulesController.cs`

```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ModulesController(IMediator mediator) : ControllerBase
{
    [HttpGet("projects/{projectId:guid}")]
    [Authorize(Policy = "ModuleRead")]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var query = new ListModulesQuery(projectId, page, pageSize, isActive, search);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ModuleRead")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetModuleByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpPost("projects/{projectId:guid}")]
    [Authorize(Policy = "ModuleCreate")]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreateModuleRequest request,
        CancellationToken ct)
    {
        var command = new CreateModuleCommand(projectId, request.Title, request.Description);
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ModuleUpdate")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateModuleRequest request,
        CancellationToken ct)
    {
        var command = new UpdateModuleCommand(id, request.Title, request.Description);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "ModuleUpdate")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeModuleStatusRequest request,
        CancellationToken ct)
    {
        var command = new ChangeModuleStatusCommand(id, request.IsActive);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ModuleDelete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await mediator.Send(new DeleteModuleCommand(id), ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}

public record CreateModuleRequest(string Title, string? Description);
public record UpdateModuleRequest(string Title, string? Description);
public record ChangeModuleStatusRequest(bool IsActive);
```

### 1.5 Authorization Policies

**Dosya**: `src/TE4IT.Infrastructure/DependencyInjection/AuthenticationRegistration.cs` içine eklenecek:

```csharp
o.AddPolicy("ModuleCreate", policy => policy.RequireAssertion(ctx =>
    ctx.User.IsInRole(RoleNames.Administrator) ||
    ctx.User.IsInRole(RoleNames.OrganizationManager) ||
    ctx.User.IsInRole(RoleNames.TeamLead) ||
    ctx.User.IsInRole(RoleNames.Trial) ||
    ctx.User.HasClaim("permission", Permissions.Module.Create)
));

o.AddPolicy("ModuleRead", policy => policy.RequireAssertion(ctx =>
    ctx.User.IsInRole(RoleNames.Administrator) ||
    ctx.User.IsInRole(RoleNames.OrganizationManager) ||
    ctx.User.IsInRole(RoleNames.TeamLead) ||
    ctx.User.IsInRole(RoleNames.Employee) ||
    ctx.User.IsInRole(RoleNames.Trial) ||
    ctx.User.HasClaim("permission", Permissions.Module.View)
));

o.AddPolicy("ModuleUpdate", policy => policy.RequireAssertion(ctx =>
    ctx.User.IsInRole(RoleNames.Administrator) ||
    ctx.User.IsInRole(RoleNames.OrganizationManager) ||
    ctx.User.IsInRole(RoleNames.TeamLead) ||
    ctx.User.IsInRole(RoleNames.Trial) ||
    ctx.User.HasClaim("permission", Permissions.Module.Update)
));

o.AddPolicy("ModuleDelete", policy => policy.RequireAssertion(ctx =>
    ctx.User.IsInRole(RoleNames.Administrator) ||
    ctx.User.IsInRole(RoleNames.OrganizationManager) ||
    ctx.User.HasClaim("permission", Permissions.Module.Delete)
));
```

**Dosya**: `src/TE4IT.Domain/Constants/Permissions.cs` içine eklenecek:

```csharp
public static class Module
{
    public const string Create = "Module.Create";
    public const string View = "Module.View";
    public const string Update = "Module.Update";
    public const string Delete = "Module.Delete";
}
```

---

## 📋 2. UseCase Feature Implementation

### 2.1 Commands

#### CreateUseCaseCommand
**Dosya**: `src/TE4IT.Application/Features/UseCases/Commands/CreateUseCase/CreateUseCaseCommand.cs`

```csharp
public sealed record CreateUseCaseCommand(
    Guid ModuleId,
    string Title,
    string? Description = null,
    string? ImportantNotes = null) : IRequest<CreateUseCaseCommandResponse>;
```

**Handler**: `CreateUseCaseCommandHandler`
- `ICurrentUser` ile creator ID alınır
- `IModuleReadRepository` ile modülün aktif olduğu kontrol edilir
- `UseCase.Create()` ile entity oluşturulur
- `IUseCaseWriteRepository.AddAsync()` ile kaydedilir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

**Validator**: `CreateUseCaseCommandValidator`
- `ModuleId`: NotEmpty
- `Title`: NotEmpty, MinLength(3), MaxLength(100)
- `Description`: MaxLength(1000) (opsiyonel)
- `ImportantNotes`: MaxLength(500) (opsiyonel)

#### UpdateUseCaseCommand
**Dosya**: `src/TE4IT.Application/Features/UseCases/Commands/UpdateUseCase/UpdateUseCaseCommand.cs`

```csharp
public sealed record UpdateUseCaseCommand(
    Guid UseCaseId,
    string Title,
    string? Description = null,
    string? ImportantNotes = null) : IRequest<bool>;
```

**Handler**: `UpdateUseCaseCommandHandler`
- `IUseCaseReadRepository.GetByIdAsync()` ile use case bulunur
- Use case null ise `false` döner
- Modülün aktif olduğu kontrol edilir
- `UseCase.UpdateTitle()`, `UpdateDescription()`, `UpdateImportantNotes()` çağrılır
- `IUseCaseWriteRepository.Update()` ile güncellenir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

#### ChangeUseCaseStatusCommand
**Dosya**: `src/TE4IT.Application/Features/UseCases/Commands/ChangeUseCaseStatus/ChangeUseCaseStatusCommand.cs`

```csharp
public sealed record ChangeUseCaseStatusCommand(
    Guid UseCaseId,
    bool IsActive) : IRequest<bool>;
```

#### DeleteUseCaseCommand
**Dosya**: `src/TE4IT.Application/Features/UseCases/Commands/DeleteUseCase/DeleteUseCaseCommand.cs`

```csharp
public sealed record DeleteUseCaseCommand(Guid UseCaseId) : IRequest<bool>;
```

### 2.2 Queries

#### GetUseCaseByIdQuery
**Dosya**: `src/TE4IT.Application/Features/UseCases/Queries/GetUseCaseById/GetUseCaseByIdQuery.cs`

```csharp
public sealed record GetUseCaseByIdQuery(Guid UseCaseId) : IRequest<UseCaseResponse>;
```

#### ListUseCasesQuery
**Dosya**: `src/TE4IT.Application/Features/UseCases/Queries/ListUseCases/ListUseCasesQuery.cs`

```csharp
public sealed record ListUseCasesQuery(
    Guid ModuleId,
    int Page = 1,
    int PageSize = 20,
    bool? IsActive = null,
    string? Search = null) : IRequest<PagedResult<UseCaseListItemResponse>>;
```

### 2.3 Responses

**Dosya**: `src/TE4IT.Application/Features/UseCases/Responses/UseCaseResponse.cs`
```csharp
public sealed class UseCaseResponse
{
    public Guid Id { get; init; }
    public Guid ModuleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ImportantNotes { get; init; }
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
}
```

**Dosya**: `src/TE4IT.Application/Features/UseCases/Responses/UseCaseListItemResponse.cs`
```csharp
public sealed class UseCaseListItemResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
    public int TaskCount { get; init; } // Aggregation
}
```

### 2.4 API Controller

**Dosya**: `src/TE4IT.API/Controllers/UseCasesController.cs`

Project Controller pattern'ini takip edecek şekilde oluşturulacak.

---

## ✅ 3. Task Feature Implementation

### 3.1 Commands

#### CreateTaskCommand
**Dosya**: `src/TE4IT.Application/Features/Tasks/Commands/CreateTask/CreateTaskCommand.cs`

```csharp
public sealed record CreateTaskCommand(
    Guid UseCaseId,
    string Title,
    TaskType TaskType,
    string? Description = null,
    string? ImportantNotes = null,
    DateTime? DueDate = null) : IRequest<CreateTaskCommandResponse>;
```

**Handler**: `CreateTaskCommandHandler`
- `ICurrentUser` ile creator ID alınır
- `IUseCaseReadRepository` ile use case'in aktif olduğu kontrol edilir
- `Task.Create()` ile entity oluşturulur (varsayılan olarak creator'a atanır)
- `DueDate` varsa `Task.UpdateDueDate()` çağrılır
- `ITaskWriteRepository.AddAsync()` ile kaydedilir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

**Validator**: `CreateTaskCommandValidator`
- `UseCaseId`: NotEmpty
- `Title`: NotEmpty, MinLength(3), MaxLength(200)
- `TaskType`: IsInEnum
- `Description`: MaxLength(2000) (opsiyonel)
- `ImportantNotes`: MaxLength(1000) (opsiyonel)
- `DueDate`: Must be after StartedDate (opsiyonel, handler'da kontrol edilebilir)

#### UpdateTaskCommand
**Dosya**: `src/TE4IT.Application/Features/Tasks/Commands/UpdateTask/UpdateTaskCommand.cs`

```csharp
public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string? Description = null,
    string? ImportantNotes = null,
    DateTime? DueDate = null) : IRequest<bool>;
```

**Handler**: `UpdateTaskCommandHandler`
- `ITaskReadRepository.GetByIdAsync()` ile task bulunur
- Task null ise `false` döner
- Use case'in aktif olduğu kontrol edilir
- `Task.UpdateTitle()`, `UpdateDescription()`, `UpdateImportantNotes()`, `UpdateDueDate()` çağrılır
- `ITaskWriteRepository.Update()` ile güncellenir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

#### ChangeTaskStateCommand
**Dosya**: `src/TE4IT.Application/Features/Tasks/Commands/ChangeTaskState/ChangeTaskStateCommand.cs`

```csharp
public sealed record ChangeTaskStateCommand(
    Guid TaskId,
    TaskState NewState) : IRequest<bool>;
```

**Handler**: `ChangeTaskStateCommandHandler`
- `ITaskReadRepository.GetByIdAsync()` ile task bulunur
- Task null ise `false` döner
- State transition kontrolü:
  - `NewState == InProgress` → `Task.Start()`
  - `NewState == Completed` → `Task.Complete()` (bloklanmış mı kontrol edilir)
  - `NewState == Cancelled` → `Task.Cancel()`
  - `NewState == NotStarted` → `Task.Revert()`
- `ITaskWriteRepository.Update()` ile güncellenir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

**Validator**: `ChangeTaskStateCommandValidator`
- `TaskId`: NotEmpty
- `NewState`: IsInEnum

#### AssignTaskCommand
**Dosya**: `src/TE4IT.Application/Features/Tasks/Commands/AssignTask/AssignTaskCommand.cs`

```csharp
public sealed record AssignTaskCommand(
    Guid TaskId,
    Guid AssigneeId) : IRequest<bool>;
```

**Handler**: `AssignTaskCommandHandler`
- `ITaskReadRepository.GetByIdAsync()` ile task bulunur
- Task null ise `false` döner
- `ICurrentUser` ile assigner ID alınır
- `Task.AssignTo(assigneeId, assignerId)` çağrılır
- `ITaskWriteRepository.Update()` ile güncellenir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

**Validator**: `AssignTaskCommandValidator`
- `TaskId`: NotEmpty
- `AssigneeId`: NotEmpty

#### DeleteTaskCommand
**Dosya**: `src/TE4IT.Application/Features/Tasks/Commands/DeleteTask/DeleteTaskCommand.cs`

```csharp
public sealed record DeleteTaskCommand(Guid TaskId) : IRequest<bool>;
```

### 3.2 Queries

#### GetTaskByIdQuery
**Dosya**: `src/TE4IT.Application/Features/Tasks/Queries/GetTaskById/GetTaskByIdQuery.cs`

```csharp
public sealed record GetTaskByIdQuery(Guid TaskId) : IRequest<TaskResponse>;
```

**Handler**: `GetTaskByIdQueryHandler`
- `ITaskReadRepository.GetByIdAsync()` ile task bulunur (Relations dahil)
- Task null ise `KeyNotFoundException` fırlatılır
- `TaskResponse` mapping yapılır (Relations dahil)

#### ListTasksQuery
**Dosya**: `src/TE4IT.Application/Features/Tasks/Queries/ListTasks/ListTasksQuery.cs`

```csharp
public sealed record ListTasksQuery(
    Guid UseCaseId,
    int Page = 1,
    int PageSize = 20,
    TaskState? State = null,
    TaskType? Type = null,
    Guid? AssigneeId = null,
    DateTime? DueDateFrom = null,
    DateTime? DueDateTo = null,
    string? Search = null) : IRequest<PagedResult<TaskListItemResponse>>;
```

**Handler**: `ListTasksQueryHandler`
- `ITaskReadRepository` üzerinden query yapılır
- `UseCaseId` ile filtreleme
- `State`, `Type`, `AssigneeId`, `DueDate` ile filtreleme (opsiyonel)
- `Search` ile title/description'a göre arama (opsiyonel)
- Pagination uygulanır
- `TaskListItemResponse` mapping yapılır

### 3.3 Responses

**Dosya**: `src/TE4IT.Application/Features/Tasks/Responses/TaskResponse.cs`
```csharp
public sealed class TaskResponse
{
    public Guid Id { get; init; }
    public Guid UseCaseId { get; init; }
    public Guid CreatorId { get; init; }
    public Guid AssigneeId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ImportantNotes { get; init; }
    public DateTime StartedDate { get; init; }
    public DateTime? DueDate { get; init; }
    public TaskType TaskType { get; init; }
    public TaskState TaskState { get; init; }
    public IReadOnlyCollection<TaskRelationResponse> Relations { get; init; } = Array.Empty<TaskRelationResponse>();
}
```

**Dosya**: `src/TE4IT.Application/Features/Tasks/Responses/TaskListItemResponse.cs`
```csharp
public sealed class TaskListItemResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public TaskType TaskType { get; init; }
    public TaskState TaskState { get; init; }
    public Guid AssigneeId { get; init; }
    public string? AssigneeName { get; init; } // User lookup
    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
}
```

**Dosya**: `src/TE4IT.Application/Features/Tasks/Responses/TaskRelationResponse.cs`
```csharp
public sealed class TaskRelationResponse
{
    public Guid Id { get; init; }
    public Guid SourceTaskId { get; init; }
    public Guid TargetTaskId { get; init; }
    public string TargetTaskTitle { get; init; } = string.Empty; // Lookup
    public TaskRelationType RelationType { get; init; }
}
```

### 3.4 API Controller

**Dosya**: `src/TE4IT.API/Controllers/TasksController.cs`

Project Controller pattern'ini takip edecek şekilde oluşturulacak.

---

## 🔗 4. TaskRelation Feature Implementation

### 4.1 Commands

#### CreateTaskRelationCommand
**Dosya**: `src/TE4IT.Application/Features/TaskRelations/Commands/CreateTaskRelation/CreateTaskRelationCommand.cs`

```csharp
public sealed record CreateTaskRelationCommand(
    Guid SourceTaskId,
    Guid TargetTaskId,
    TaskRelationType RelationType) : IRequest<bool>;
```

**Handler**: `CreateTaskRelationCommandHandler`
- `ITaskReadRepository` ile source ve target task'lar bulunur
- SourceTaskId != TargetTaskId kontrolü
- Döngüsel bağımlılık kontrolü (opsiyonel, uygulama seviyesinde)
- `TaskRelation.Create()` ile entity oluşturulur
- `Task.AddRelation()` ile task'a eklenir
- `ITaskWriteRepository.Update()` ile güncellenir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

**Validator**: `CreateTaskRelationCommandValidator`
- `SourceTaskId`: NotEmpty
- `TargetTaskId`: NotEmpty, NotEqual(SourceTaskId)
- `RelationType`: IsInEnum

#### DeleteTaskRelationCommand
**Dosya**: `src/TE4IT.Application/Features/TaskRelations/Commands/DeleteTaskRelation/DeleteTaskRelationCommand.cs`

```csharp
public sealed record DeleteTaskRelationCommand(
    Guid TaskId,
    Guid RelationId) : IRequest<bool>;
```

**Handler**: `DeleteTaskRelationCommandHandler`
- `ITaskReadRepository.GetByIdAsync()` ile task bulunur (Relations dahil)
- Task null ise `false` döner
- Relation bulunur
- `Task.RemoveRelation()` çağrılır
- `ITaskWriteRepository.Update()` ile güncellenir
- `IUnitOfWork.SaveChangesAsync()` ile commit edilir

### 4.2 Queries

#### GetTaskRelationsQuery
**Dosya**: `src/TE4IT.Application/Features/TaskRelations/Queries/GetTaskRelations/GetTaskRelationsQuery.cs`

```csharp
public sealed record GetTaskRelationsQuery(Guid TaskId) : IRequest<IReadOnlyCollection<TaskRelationResponse>>;
```

**Handler**: `GetTaskRelationsQueryHandler`
- `ITaskReadRepository.GetByIdAsync()` ile task bulunur (Relations dahil)
- Task null ise `KeyNotFoundException` fırlatılır
- Relations mapping yapılır

### 4.3 API Controller

**Dosya**: `src/TE4IT.API/Controllers/TaskRelationsController.cs`

```csharp
[ApiController]
[Route("api/v1/tasks/{taskId:guid}/relations")]
[Authorize]
public class TaskRelationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "TaskRead")]
    public async Task<IActionResult> GetRelations(Guid taskId, CancellationToken ct)
    {
        var query = new GetTaskRelationsQuery(taskId);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "TaskRelationCreate")]
    public async Task<IActionResult> Create(
        Guid taskId,
        [FromBody] CreateTaskRelationRequest request,
        CancellationToken ct)
    {
        var command = new CreateTaskRelationCommand(taskId, request.TargetTaskId, request.RelationType);
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{relationId:guid}")]
    [Authorize(Policy = "TaskRelationDelete")]
    public async Task<IActionResult> Delete(
        Guid taskId,
        Guid relationId,
        CancellationToken ct)
    {
        var ok = await mediator.Send(new DeleteTaskRelationCommand(taskId, relationId), ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}

public record CreateTaskRelationRequest(Guid TargetTaskId, TaskRelationType RelationType);
```

---

## 🔐 5. Authorization Policies

### 5.1 Permissions Constants

**Dosya**: `src/TE4IT.Domain/Constants/Permissions.cs` içine eklenecek:

```csharp
public static class Module
{
    public const string Create = "Module.Create";
    public const string View = "Module.View";
    public const string Update = "Module.Update";
    public const string Delete = "Module.Delete";
}

public static class UseCase
{
    public const string Create = "UseCase.Create";
    public const string View = "UseCase.View";
    public const string Update = "UseCase.Update";
    public const string Delete = "UseCase.Delete";
}

public static class Task
{
    public const string Create = "Task.Create";
    public const string View = "Task.View";
    public const string Update = "Task.Update";
    public const string Assign = "Task.Assign";
    public const string StateChange = "Task.StateChange";
    public const string Delete = "Task.Delete";
}

public static class TaskRelation
{
    public const string Create = "TaskRelation.Create";
    public const string Delete = "TaskRelation.Delete";
}
```

### 5.2 Policy Definitions

**Dosya**: `src/TE4IT.Infrastructure/DependencyInjection/AuthenticationRegistration.cs` içine eklenecek:

Tüm Module, UseCase, Task ve TaskRelation policy'leri Project policy'leri gibi tanımlanacak.

---

## 📊 6. Repository Extensions

### 6.1 Module Repository Extensions

**Dosya**: `src/TE4IT.Application/Abstractions/Persistence/Repositories/Modules/IModuleReadRepository.cs`

```csharp
public interface IModuleReadRepository : IReadRepository<Module>
{
    Task<int> CountByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Module>> GetByProjectIdAsync(
        Guid projectId,
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default);
}
```

**Implementation**: `src/TE4IT.Persistence/TaskManagement/Repositories/Modules/ModuleReadRepository.cs`

### 6.2 UseCase Repository Extensions

**Dosya**: `src/TE4IT.Application/Abstractions/Persistence/Repositories/UseCases/IUseCaseReadRepository.cs`

```csharp
public interface IUseCaseReadRepository : IReadRepository<UseCase>
{
    Task<int> CountByModuleAsync(Guid moduleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UseCase>> GetByModuleIdAsync(
        Guid moduleId,
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default);
}
```

### 6.3 Task Repository Extensions

**Dosya**: `src/TE4IT.Application/Abstractions/Persistence/Repositories/Tasks/ITaskReadRepository.cs`

```csharp
public interface ITaskReadRepository : IReadRepository<Task>
{
    Task<int> CountByUseCaseAsync(Guid useCaseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Task>> GetByUseCaseIdAsync(
        Guid useCaseId,
        TaskState? state = null,
        TaskType? type = null,
        Guid? assigneeId = null,
        DateTime? dueDateFrom = null,
        DateTime? dueDateTo = null,
        string? search = null,
        CancellationToken cancellationToken = default);
    Task<Task?> GetByIdWithRelationsAsync(Guid taskId, CancellationToken cancellationToken = default);
}
```

---

## 🧪 7. Test Gereksinimleri

### 7.1 Unit Tests

Her feature için:
- Command Handler testleri
- Query Handler testleri
- Validator testleri
- Domain entity testleri (Module, UseCase, Task)

### 7.2 Integration Tests

- Repository testleri
- API endpoint testleri

---

## 📝 8. Implementasyon Öncelik Sırası

1. **Faz 1: Module Feature**
   - Commands (Create, Update, ChangeStatus, Delete)
   - Queries (GetById, List)
   - Responses
   - API Controller
   - Authorization Policies

2. **Faz 2: UseCase Feature**
   - Commands (Create, Update, ChangeStatus, Delete)
   - Queries (GetById, List)
   - Responses
   - API Controller
   - Authorization Policies

3. **Faz 3: Task Feature**
   - Commands (Create, Update, ChangeState, Assign, Delete)
   - Queries (GetById, List)
   - Responses
   - API Controller
   - Authorization Policies

4. **Faz 4: TaskRelation Feature**
   - Commands (Create, Delete)
   - Queries (GetRelations)
   - API Controller
   - Authorization Policies

5. **Faz 5: Repository Extensions**
   - Module repository extensions
   - UseCase repository extensions
   - Task repository extensions

6. **Faz 6: Tests**
   - Unit tests
   - Integration tests

---

## 🔍 9. Önemli Notlar

1. **Business Rules**: Tüm handler'larda business rule kontrolleri yapılmalı
2. **Error Handling**: Domain exceptions kullanılmalı (BusinessRuleViolationException, ResourceNotFoundException, vb.)
3. **Validation**: FluentValidation ile tüm request'ler validate edilmeli
4. **Authorization**: Policy-based authorization kullanılmalı
5. **Pagination**: Tüm listeleme endpoint'lerinde pagination olmalı
6. **Soft Delete**: Soft delete kullanılıyorsa repository'lerde filtrelenmeli
7. **Domain Events**: Task state değişiklikleri ve atamalar domain event fırlatmalı
8. **Mapping**: AutoMapper veya manual mapping kullanılabilir

---

## 📚 10. Referanslar

- Project Feature implementasyonu (pattern referansı)
- Domain entities (Module, UseCase, Task, TaskRelation)
- Persistence repositories (mevcut)
- EF Core configurations (mevcut)

