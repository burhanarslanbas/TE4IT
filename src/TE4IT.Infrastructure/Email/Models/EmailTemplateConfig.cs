namespace TE4IT.Infrastructure.Email.Models;

/// <summary>
/// Email template konfigürasyonu
/// </summary>
public class EmailTemplateConfig
{
    public Dictionary<string, EmailTemplateInfo> Templates { get; set; } = new();
}

/// <summary>
/// Email template bilgileri
/// </summary>
public class EmailTemplateInfo
{
    public string Subject { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Dictionary<string, string> DefaultVariables { get; set; } = new();
}
