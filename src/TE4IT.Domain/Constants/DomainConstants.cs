namespace TE4IT.Domain.Constants;

/// <summary>
/// Domain sabitleri ve iş kuralları limitleri
/// </summary>
public static class DomainConstants
{
    // Project sabitleri
    public const int MaxProjectTitleLength = 100;
    public const int MinProjectTitleLength = 3;
    public const int MaxProjectDescriptionLength = 1000;
    public const int MaxTeamMembersPerProject = 50;

    // Module sabitleri
    public const int MaxModuleTitleLength = 100;
    public const int MinModuleTitleLength = 3;
    public const int MaxModuleDescriptionLength = 1000;
    public const int MaxModulesPerProject = 100;

    // UseCase sabitleri
    public const int MaxUseCaseTitleLength = 100;
    public const int MinUseCaseTitleLength = 3;
    public const int MaxUseCaseDescriptionLength = 1000;
    public const int MaxUseCaseImportantNotesLength = 500;
    public const int MaxUseCasesPerModule = 200;

    // Task sabitleri
    public const int MaxTaskTitleLength = 200;
    public const int MinTaskTitleLength = 3;
    public const int MaxTaskDescriptionLength = 2000;
    public const int MaxTaskImportantNotesLength = 1000;
    public const int MaxTaskCompletionNoteLength = 2000;
    public const int MaxTasksPerUseCase = 500;
    public const int MaxTaskRelationsPerTask = 20;

    // User sabitleri
    public const int MaxUserFullNameLength = 100;
    public const int MinUserFullNameLength = 2;
    public const int MaxEmailLength = 254;
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 128;

    // Tarih sabitleri
    public const int MaxProjectDurationDays = 3650; // 10 yıl
    public const int MinTaskDurationHours = 1;
    public const int MaxTaskDurationDays = 365;

    // İş kuralları sabitleri
    public const int MaxConcurrentActiveProjectsPerUser = 10;
    public const int MaxConcurrentActiveTasksPerUser = 50;
    public const int MaxTaskDependencyDepth = 5; // Görev bağımlılığı maksimum derinliği

    // Dosya ve medya sabitleri
    public const int MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
    public const int MaxAttachmentsPerTask = 10;
    public static readonly string[] AllowedFileExtensions = new[] { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".png", ".gif" };

    // Bildirim sabitleri
    public const int MaxNotificationRetryAttempts = 3;
    public const int NotificationRetryDelayMinutes = 5;

    // Cache sabitleri
    public const int DefaultCacheExpirationMinutes = 30;
    public const int UserSessionTimeoutMinutes = 480; // 8 saat

    // Validation mesajları
    public const string RequiredFieldMessage = "Bu alan zorunludur.";
    public const string InvalidEmailMessage = "Geçerli bir e-posta adresi giriniz.";
    public const string InvalidDateRangeMessage = "Başlangıç tarihi bitiş tarihinden önce olmalıdır.";
    public const string TaskDependencyCycleMessage = "Görev bağımlılığında döngü oluşturamazsınız.";
    public const string AccessDeniedMessage = "Bu işlem için yetkiniz bulunmamaktadır.";
    public const string ResourceNotFoundMessage = "Aranan kaynak bulunamadı.";
    public const string DuplicateResourceMessage = "Bu kaynak zaten mevcut.";
}
