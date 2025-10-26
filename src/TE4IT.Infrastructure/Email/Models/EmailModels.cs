namespace TE4IT.Infrastructure.Email.Models;

/// <summary>
/// Email template veri modeli
/// </summary>
public class EmailTemplate
{
    public string Subject { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public Dictionary<string, object> Variables { get; set; } = new();
}

/// <summary>
/// Email gönderim veri modeli
/// </summary>
public class EmailData
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string? TextBody { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
}
