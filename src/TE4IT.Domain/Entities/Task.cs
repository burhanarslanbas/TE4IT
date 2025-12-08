using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Events;
using TE4IT.Domain.Exceptions.Tasks;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Domain.Entities;

public class Task : AggregateRoot
{
    public Guid UseCaseId { get; private set; } // Bağlı olduğu UseCase
    public UserId CreatorId { get; private set; } = null!; // Oluşturan Kişi
    public UserId? AssigneeId { get; private set; } // Atanan Kişi (başlangıçta null olabilir)
    public string Title { get; private set; } = string.Empty; // Başlık
    public string? Description { get; private set; } // Açıklama
    public string? ImportantNotes { get; private set; } // Önemli Notlar
    public DateTime? StartedDate { get; private set; } // Başlangıç Tarihi (sadece başlatıldığında set edilir)
    public DateTime? DueDate { get; private set; } // Bitiş Tarihi
    public TaskType TaskType { get; private set; } // Görev tipi (Özellik, Dokümantasyon, Test, Hata Düzeltme vb.)
    public TaskState TaskState { get; private set; } = TaskState.NotStarted; // Başlangıç durumu NotStarted olarak ayarlandı
    private readonly List<TaskRelation> _relations = new();
    public IReadOnlyCollection<TaskRelation> Relations => _relations.AsReadOnly(); // İlgili Görevler (Bağımlı görevler)

    /// <summary>
    /// Görev oluşturur
    /// </summary>
    private Task() { }

    public static Task Create(Guid useCaseId, UserId creatorId, string title, TaskType taskType, string? description = null, string? importantNotes = null)
    {
        ValidateTitle(title);
        ValidateDescription(description);
        ValidateImportantNotes(importantNotes);

        var task = new Task
        {
            UseCaseId = useCaseId,
            CreatorId = creatorId,
            AssigneeId = null, // Başlangıçta atanmamış
            Title = title,
            Description = description,
            ImportantNotes = importantNotes,
            StartedDate = null, // Başlangıçta başlatılmamış
            TaskType = taskType,
            TaskState = TaskState.NotStarted
        };

        return task;
    }

    /// <summary>
    /// Görevi başlatır
    /// </summary>
    public void Start()
    {
        if (TaskState != TaskState.NotStarted)
            throw new InvalidTaskStateTransitionException(TaskState, TaskState.InProgress);

        if (AssigneeId == null)
            throw new InvalidOperationException("Görev başlatılmadan önce bir kişiye atanmış olmalıdır.");

        TaskState = TaskState.InProgress;
        StartedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new TaskStartedEvent(Id, AssigneeId.Value, UseCaseId, Title, TaskType));
    }

    /// <summary>
    /// Görevi tamamlar
    /// </summary>
    public void Complete()
    {
        if (TaskState != TaskState.InProgress)
            throw new InvalidTaskStateTransitionException(TaskState, TaskState.Completed);

        if (AssigneeId == null)
            throw new InvalidOperationException("Atanmamış görev tamamlanamaz.");

        TaskState = TaskState.Completed;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new TaskCompletedEvent(Id, AssigneeId.Value, UseCaseId, Title, TaskType));
    }

    /// <summary>
    /// Görevi iptal eder
    /// </summary>
    public void Cancel()
    {
        if (TaskState == TaskState.Completed)
            throw new InvalidTaskStateTransitionException(TaskState, TaskState.Cancelled);

        TaskState = TaskState.Cancelled;
        UpdatedDate = DateTime.UtcNow;

        if (AssigneeId != null)
            AddDomainEvent(new TaskCancelledEvent(Id, AssigneeId.Value, UseCaseId, Title, TaskType));
    }

    /// <summary>
    /// Görevi geri alır (NotStarted durumuna)
    /// </summary>
    public void Revert()
    {
        if (TaskState == TaskState.Completed)
            throw new InvalidTaskStateTransitionException(TaskState, TaskState.NotStarted);

        TaskState = TaskState.NotStarted;
        UpdatedDate = DateTime.UtcNow;

        if (AssigneeId != null)
            AddDomainEvent(new TaskRevertedEvent(Id, AssigneeId.Value, UseCaseId, Title, TaskType));
    }

    /// <summary>
    /// Görevi bir kişiye atar
    /// </summary>
    public void AssignTo(UserId assigneeId, UserId assignerId)
    {
        if ((Guid)assigneeId == Guid.Empty)
            throw new ArgumentException("Atanan kişi ID'si boş olamaz.", nameof(assigneeId));

        AssigneeId = assigneeId;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new TaskAssignedEvent(Id, assigneeId.Value, assignerId.Value, UseCaseId, Title, TaskType, DueDate));
    }

    /// <summary>
    /// Görev başlığını günceller
    /// </summary>
    public void UpdateTitle(string title)
    {
        ValidateTitle(title);
        Title = title;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Görev açıklamasını günceller
    /// </summary>
    public void UpdateDescription(string? description)
    {
        ValidateDescription(description);
        Description = description;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Önemli notları günceller
    /// </summary>
    public void UpdateImportantNotes(string? importantNotes)
    {
        ValidateImportantNotes(importantNotes);
        ImportantNotes = importantNotes;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Bitiş tarihini günceller
    /// </summary>
    public void UpdateDueDate(DateTime? dueDate)
    {
        if (dueDate.HasValue && StartedDate.HasValue && dueDate.Value < StartedDate.Value)
            throw new ArgumentException(DomainConstants.InvalidDateRangeMessage, nameof(dueDate));

        DueDate = dueDate;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Görev bağımlılığı ekler
    /// </summary>
    public void AddRelation(TaskRelation relation)
    {
        if (relation == null)
            throw new ArgumentNullException(nameof(relation));

        if (relation.SourceTaskId != Id)
            throw new InvalidOperationException("Bağımlılık bu göreve ait değil.");

        _relations.Add(relation);
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Görev bağımlılığını kaldırır
    /// </summary>
    public void RemoveRelation(Guid relationId)
    {
        var relation = _relations.FirstOrDefault(r => r.Id == relationId);
        if (relation != null)
        {
            _relations.Remove(relation);
            UpdatedDate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Görevin blokladığı görevleri getirir
    /// </summary>
    public IEnumerable<TaskRelation> GetBlockingRelations()
    {
        return Relations.Where(r => r.RelationType == TaskRelationType.Blocks);
    }

    /// <summary>
    /// Görevin bağımlı olduğu görevleri getirir
    /// </summary>
    public IEnumerable<TaskRelation> GetDependentRelations()
    {
        return Relations.Where(r => r.RelationType != TaskRelationType.Blocks);
    }

    /// <summary>
    /// Görevin tamamlanabilir olup olmadığını kontrol eder
    /// </summary>
    public bool CanBeCompleted()
    {
        return TaskState == TaskState.InProgress && !GetBlockingRelations().Any();
    }

    /// <summary>
    /// Görevin gecikmiş olup olmadığını kontrol eder
    /// </summary>
    public bool IsOverdue()
    {
        return DueDate.HasValue && DueDate.Value < DateTime.UtcNow && TaskState != TaskState.Completed;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(DomainConstants.RequiredFieldMessage, nameof(title));

        if (title.Length < DomainConstants.MinTaskTitleLength)
            throw new ArgumentException($"Görev başlığı en az {DomainConstants.MinTaskTitleLength} karakter olmalıdır.", nameof(title));

        if (title.Length > DomainConstants.MaxTaskTitleLength)
            throw new ArgumentException($"Görev başlığı en fazla {DomainConstants.MaxTaskTitleLength} karakter olabilir.", nameof(title));
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length > DomainConstants.MaxTaskDescriptionLength)
            throw new ArgumentException($"Görev açıklaması en fazla {DomainConstants.MaxTaskDescriptionLength} karakter olabilir.", nameof(description));
    }

    private static void ValidateImportantNotes(string? importantNotes)
    {
        if (importantNotes != null && importantNotes.Length > DomainConstants.MaxTaskImportantNotesLength)
            throw new ArgumentException($"Önemli notlar en fazla {DomainConstants.MaxTaskImportantNotesLength} karakter olabilir.", nameof(importantNotes));
    }
}

