using System.Web;
using TE4IT.Domain.Services;

namespace TE4IT.Infrastructure.Education.Services;

/// <summary>
/// Video URL'leri için platform tespiti ve embed URL üretimi yapan domain servisi implementasyonu.
/// </summary>
public sealed class VideoUrlService : IVideoUrlService
{
    public string? GetEmbedUrl(string originalUrl)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
            return null;

        // YouTube: https://www.youtube.com/watch?v=VIDEO_ID
        if (originalUrl.Contains("youtube.com/watch?v="))
        {
            var videoId = ExtractYouTubeVideoId(originalUrl);
            if (!string.IsNullOrEmpty(videoId))
            {
                return $"https://www.youtube.com/embed/{videoId}";
            }
        }
        
        // YouTube Short: https://youtu.be/VIDEO_ID
        if (originalUrl.Contains("youtu.be/"))
        {
            var videoId = originalUrl.Split('/').Last().Split('?').First();
            if (!string.IsNullOrEmpty(videoId))
            {
                return $"https://www.youtube.com/embed/{videoId}";
            }
        }

        // Vimeo: https://vimeo.com/VIDEO_ID
        if (originalUrl.Contains("vimeo.com/"))
        {
            var videoId = originalUrl.Split('/').Last().Split('?').First();
            if (!string.IsNullOrEmpty(videoId))
            {
                return $"https://player.vimeo.com/video/{videoId}";
            }
        }

        return originalUrl; // Desteklenmeyen platformlar için orijinal URL
    }

    public string? DetectPlatform(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        if (url.Contains("youtube.com") || url.Contains("youtu.be"))
            return "youtube";
        
        if (url.Contains("vimeo.com"))
            return "vimeo";
        
        return "unknown";
    }

    public bool IsValidVideoUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        if (uri.Scheme != "https" && uri.Scheme != "http")
            return false;

        return DetectPlatform(url) != "unknown";
    }

    private string ExtractYouTubeVideoId(string url)
    {
        try
        {
            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);
            return query["v"] ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}

