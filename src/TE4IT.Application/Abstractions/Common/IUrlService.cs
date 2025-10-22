namespace TE4IT.Application.Abstractions.Common;

/// <summary>
/// URL oluşturma ve yönetimi için servis
/// </summary>
public interface IUrlService
{
    /// <summary>
    /// Frontend uygulamasının base URL'ini döner
    /// </summary>
    string GetFrontendUrl();
    
    /// <summary>
    /// Backend API'nin base URL'ini döner
    /// </summary>
    string GetBackendUrl();
}

