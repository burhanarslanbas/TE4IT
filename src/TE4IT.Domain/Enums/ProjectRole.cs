namespace TE4IT.Domain.Enums;

/// <summary>
/// Proje rolleri - kullanıcının belirli bir projedeki rolünü belirler
/// </summary>
public enum ProjectRole
{
    Viewer = 1,  // Sadece görüntüleme yetkisi
    Member = 2,  // Düzenleme yetkisi
    Owner = 5    // Proje sahibi (tam yetki)
}

