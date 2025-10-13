Task Management — Class Diagram (Mermaid)

Aşağıdaki diyagram yalnızca Görev Yönetimi alanını (Task, TaskRelation, UseCase ve ilgili enumlar/VO) kapsamaktadır.

```mermaid
classDiagram
    direction TB

    class UserId {
        +Guid Value
    }

    class UseCase {
        +Guid ModuleId
        +UserId CreatorId
        +string Title
        +string Description
        +string ImportantNotes
        +DateTime StartedDate
        +bool IsActive
    }

    class Task {
        +Guid UseCaseId
        +UserId CreatorId
        +UserId AssigneeId
        +string Title
        +string Description
        +string ImportantNotes
        +DateTime StartedDate
        +DateTime DueDate
        +TaskType TaskType
        +TaskState TaskState
    }

    class TaskRelation {
        +Guid SourceTaskId
        +Guid TargetTaskId
        +TaskRelationType RelationType
    }

    %% Associations
    UseCase "1" o-- "*" Task : contains
    Task "1" o-- "*" TaskRelation : relations
    TaskRelation --> Task : source
    TaskRelation --> Task : target

    %% Value Object references
    Task --> UserId : CreatorId
    Task --> UserId : AssigneeId
    UseCase --> UserId : CreatorId

    %% Enums (temsilî)
    class TaskState {
        NotStarted
        InProgress
        Completed
        Cancelled
    }

    class TaskType {
        Documentation
        Feature
        Test
        Bug
    }

    class TaskRelationType {
        Blocks
        RelatesTo
        Fixes
        Duplicates
    }

    Task --> TaskType
    Task --> TaskState
    TaskRelation --> TaskRelationType
```

Notlar
- `Task` durum geçişleri: NotStarted → InProgress → Completed; InProgress → Cancelled; Completed için geri alma `Revert()` ile kontrol edilir.
- `TaskRelation` ile döngüsel bağımlılık uygulama seviyesinde engellenmelidir.

