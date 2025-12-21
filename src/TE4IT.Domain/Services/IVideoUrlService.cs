namespace TE4IT.Domain.Services;

/// <summary>
/// Video URL'leri için platform tespiti ve embed URL üretimi yapan domain servisi sözleşmesi.
/// Implementasyonu Infrastructure katmanında yapılmalıdır.
/// </summary>
public interface IVideoUrlService
{
    string? GetEmbedUrl(string originalUrl);
    string? DetectPlatform(string url);
    bool IsValidVideoUrl(string url);
}

