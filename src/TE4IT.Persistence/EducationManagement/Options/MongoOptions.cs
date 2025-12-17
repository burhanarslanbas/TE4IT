namespace TE4IT.Persistence.EducationManagement.Options;

public sealed class MongoOptions
{
    public string ConnectionString { get; set; } = default!;
    public string DatabaseName { get; set; } = "te4it_education";
}

