using TE4IT.Domain.Entities;
using TE4IT.Domain.ValueObjects;
using TaskEntity = TE4IT.Domain.Entities.Task;

namespace TE4IT.Domain.Services;

/// <summary>
/// Kullanıcı yetki servisi - kullanıcıların proje ve görev erişim yetkilerini kontrol eder
/// </summary>
public interface IUserPermissionService
{
    /// <summary>
    /// Kullanıcının projeye erişim yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="project">Proje</param>
    /// <returns>Erişim yetkisi varsa true, aksi halde false</returns>
    bool CanAccessProject(UserId userId, Project project);

    /// <summary>
    /// Kullanıcının projeyi düzenleme yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="project">Proje</param>
    /// <returns>Düzenleme yetkisi varsa true, aksi halde false</returns>
    bool CanEditProject(UserId userId, Project project);

    /// <summary>
    /// Kullanıcının projeyi silme yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="project">Proje</param>
    /// <returns>Silme yetkisi varsa true, aksi halde false</returns>
    bool CanDeleteProject(UserId userId, Project project);

    /// <summary>
    /// Kullanıcının görevi görüntüleme yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="task">Görev</param>
    /// <returns>Görüntüleme yetkisi varsa true, aksi halde false</returns>
    bool CanViewTask(UserId userId, TaskEntity task);

    /// <summary>
    /// Kullanıcının görevi düzenleme yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="task">Görev</param>
    /// <returns>Düzenleme yetkisi varsa true, aksi halde false</returns>
    bool CanEditTask(UserId userId, TaskEntity task);

    /// <summary>
    /// Kullanıcının görevi silme yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="task">Görev</param>
    /// <returns>Silme yetkisi varsa true, aksi halde false</returns>
    bool CanDeleteTask(UserId userId, TaskEntity task);

    /// <summary>
    /// Kullanıcının görevi atama yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="task">Görev</param>
    /// <returns>Atama yetkisi varsa true, aksi halde false</returns>
    bool CanAssignTask(UserId userId, TaskEntity task);

    /// <summary>
    /// Kullanıcının modül oluşturma yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="project">Proje</param>
    /// <returns>Modül oluşturma yetkisi varsa true, aksi halde false</returns>
    bool CanCreateModule(UserId userId, Project project);

    /// <summary>
    /// Kullanıcının use case oluşturma yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="module">Modül</param>
    /// <returns>Use case oluşturma yetkisi varsa true, aksi halde false</returns>
    bool CanCreateUseCase(UserId userId, Module module);

    /// <summary>
    /// Kullanıcının görev oluşturma yetkisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="useCase">Use case</param>
    /// <returns>Görev oluşturma yetkisi varsa true, aksi halde false</returns>
    bool CanCreateTask(UserId userId, UseCase useCase);

    /// <summary>
    /// Kullanıcının proje rolünü getirir
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <param name="project">Proje</param>
    /// <returns>Proje rolü</returns>
    ProjectRole GetUserProjectRole(UserId userId, Project project);

    /// <summary>
    /// Kullanıcının sistem yöneticisi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <returns>Sistem yöneticisi ise true, aksi halde false</returns>
    bool IsSystemAdministrator(UserId userId);
}

/// <summary>
/// Proje rolleri
/// </summary>
public enum ProjectRole
{
    None = 0,
    Viewer = 1,
    Member = 2,
    Manager = 3,
    Admin = 4,
    Owner = 5
}
