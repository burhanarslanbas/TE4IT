using TE4IT.Domain.Entities;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Domain.Specifications;

/// <summary>
/// Kullanıcının projeye erişebilir olup olmadığını kontrol eden specification
/// </summary>
public class UserCanAccessProjectSpecification
{
    private readonly IUserPermissionService _permissionService;

    public UserCanAccessProjectSpecification(IUserPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Kullanıcının projeye erişebilir olup olmadığını kontrol eder
    /// </summary>
    /// <param name="userId">Kullanıcı kimliği</param>
    /// <param name="project">Proje</param>
    /// <returns>Erişebilirse true, aksi halde false</returns>
    public bool IsSatisfiedBy(UserId userId, Project project)
    {
        if (project == null)
            return false;

        // Sistem yöneticisi her projeye erişebilir
        if (_permissionService.IsSystemAdministrator(userId))
            return true;

        // Proje oluşturan kişi erişebilir
        if (project.CreatorId == userId)
            return true;

        // Kullanıcının proje erişim yetkisi var mı kontrol et
        return _permissionService.CanAccessProject(userId, project);
    }

    /// <summary>
    /// Kullanıcının projeye erişememe sebebini getirir
    /// </summary>
    /// <param name="userId">Kullanıcı kimliği</param>
    /// <param name="project">Proje</param>
    /// <returns>Erişememe sebebi</returns>
    public string GetReason(UserId userId, Project project)
    {
        if (project == null)
            return "Proje bulunamadı.";

        if (_permissionService.IsSystemAdministrator(userId))
            return "Sistem yöneticisi erişimi.";

        if (project.CreatorId == userId)
            return "Proje sahibi erişimi.";

        if (_permissionService.CanAccessProject(userId, project))
            return "Proje üyesi erişimi.";

        return "Proje erişim yetkisi bulunmuyor.";
    }
}
