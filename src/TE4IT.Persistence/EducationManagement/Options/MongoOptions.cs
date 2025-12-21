namespace TE4IT.Persistence.EducationManagement.Options;

/// <summary>
/// MongoDB bağlantı ayarları.
/// <para>
/// <c>ConnectionString</c> alanı, Atlas dokümantasyonundaki gibi
/// tam bağlantı adresini içermelidir. Örnek:
/// <c>mongodb+srv://burhan:&lt;url-encoded-password&gt;@te4it-education-dev.jpzdt0e.mongodb.net/?appName=te4it-education-dev</c>
/// </para>
/// <para>
/// Parolada <c>@</c>, <c>:</c>, <c>/</c> gibi özel karakterler varsa
/// URL encode edilmelidir (bkz. MongoDB Atlas "Special characters in connection string password").
/// </para>
/// </summary>
public sealed class MongoOptions
{
    /// <summary>
    /// Tam MongoDB bağlantı dizesi (mongodb veya mongodb+srv URI).
    /// </summary>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Kullanılacak veritabanı adı. Atlas connection string'inde database adı verilmemişse
    /// burada belirtilebilir (ör: "te4it_education").
    /// </summary>
    public string DatabaseName { get; set; } = "te4it-education-dev";
}

