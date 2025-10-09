namespace TE4IT.Domain.Enums;

public enum TaskState
{
    NotStarted = 1, // Başlanmadı
    InProgress, // Devam Ediyor
    Completed, // Tamamlandı
    Cancelled// İptal Edildi
}