using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TE4IT.API;
using TE4IT.Application.Features.Auth.Commands.Login;
using TE4IT.Application.Features.Auth.Commands.Register;
using TE4IT.Application.Features.Modules.Commands.CreateModule;
using TE4IT.Application.Features.Projects.Commands.CreateProject;
using TE4IT.Application.Features.Tasks.Commands.CreateTask;
using TE4IT.Application.Features.Tasks.Responses;
using TE4IT.Application.Features.UseCases.Commands.CreateUseCase;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Enums;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Identity;
using TE4IT.Persistence.TaskManagement.ServiceRegistrations;

namespace TE4IT.Tests.API.Common;

/// <summary>
/// API testleri için base class - WebApplicationFactory kullanarak gerçek HTTP istekleri yapar
/// </summary>
public abstract class ApiTestBase : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime, IDisposable
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly AppDbContext DbContext;
    protected readonly IServiceScope ServiceScope;
    private readonly string _databaseName;

    protected ApiTestBase(WebApplicationFactory<Program> factory)
    {
        // Her test instance'ı için benzersiz database adı oluştur
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Tüm DbContextOptions kayıtlarını kaldır
                var dbContextOptionsDescriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                               (d.ServiceType.IsGenericType && 
                                d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                    .ToList();

                foreach (var descriptor in dbContextOptionsDescriptors)
                {
                    services.Remove(descriptor);
                }

                // AppDbContext factory kayıtlarını kaldır
                var appDbContextFactoryDescriptors = services
                    .Where(d => d.ServiceType == typeof(Func<AppDbContext>) ||
                               (d.ServiceType.IsGenericType && d.ServiceType.GetGenericTypeDefinition() == typeof(Func<>)) ||
                               d.ServiceType == typeof(AppDbContext))
                    .ToList();

                foreach (var descriptor in appDbContextFactoryDescriptors)
                {
                    services.Remove(descriptor);
                }

                // In-Memory database ile AddRelational'ı manuel çağır
                // (ApplicationServiceConfiguration'da Testing ortamında atlanıyor)
                services.AddRelational(opt =>
                {
                    opt.UseInMemoryDatabase(_databaseName);
                });
            });

            // Test ortamını belirle (Configuration'dan önce)
            builder.UseEnvironment("Testing");

            // Configuration'ı override et
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Test ortamında JWT ve diğer ayarları ekle
                // AddInMemoryCollection en son eklenen kaynak olduğu için öncelikli olacak
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Issuer", "https://localhost:5001" },
                    { "Jwt:Audience", "te4it-api" },
                    { "Jwt:SigningKey", "ToAyJiE3OPBfbv7GN0elgLNxxEwmXtd+7EyC76yxiss=" },
                    { "ConnectionStrings:Pgsql", "" }, // Boş connection string - AddRelational atlanacak
                    { "ASPNETCORE_ENVIRONMENT", "Testing" }
                });
            });
        });

        Client = Factory.CreateClient();
        ServiceScope = Factory.Services.CreateScope();
        DbContext = ServiceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// Her test method'undan önce çağrılır - test verilerini seed eder
    /// </summary>
    public async System.Threading.Tasks.Task InitializeAsync()
    {
        await SeedTestDataAsync();
    }

    /// <summary>
    /// Her test method'undan sonra çağrılır
    /// </summary>
    public System.Threading.Tasks.Task DisposeAsync()
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }

    /// <summary>
    /// Test veritabanını hazırlar (roller, varsayılan kullanıcılar, vb.)
    /// </summary>
    protected virtual async System.Threading.Tasks.Task SeedTestDataAsync()
    {
        // Rolleri seed et - DbContext üzerinden doğrudan ekle (RoleManager'dan daha güvenilir)
        var roles = new[]
        {
            RoleNames.Administrator,
            RoleNames.OrganizationManager,
            RoleNames.TeamLead,
            RoleNames.Employee,
            RoleNames.Trainer,
            RoleNames.Customer,
            RoleNames.Trial
        };

        foreach (var roleName in roles)
        {
            var normalizedName = roleName.ToUpperInvariant();
            var existingRole = await DbContext.Roles
                .FirstOrDefaultAsync(r => r.NormalizedName == normalizedName);
            
            if (existingRole == null)
            {
                var role = new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = normalizedName
                };
                DbContext.Roles.Add(role);
            }
        }

        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Kullanıcı kaydı yapar ve token döndürür
    /// </summary>
    protected async System.Threading.Tasks.Task<string> RegisterAndGetTokenAsync(string email = "test@example.com", string password = "Test123!@#", string userName = "testuser")
    {
        var registerRequest = new RegisterCommand(userName, email, password);
        var registerResponse = await Client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        // Login yap ve token al
        var loginRequest = new LoginCommand(email, password);
        var loginResponse = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginCommandResponse>();
        return loginResult?.AccessToken ?? throw new InvalidOperationException("Token alınamadı");
    }

    /// <summary>
    /// Mevcut kullanıcı ile login yapar ve token döndürür
    /// </summary>
    protected async System.Threading.Tasks.Task<string> LoginAndGetTokenAsync(string email, string password)
    {
        var loginRequest = new LoginCommand(email, password);
        var loginResponse = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginCommandResponse>();
        return loginResult?.AccessToken ?? throw new InvalidOperationException("Token alınamadı");
    }

    /// <summary>
    /// HTTP client'a authorization header ekler
    /// </summary>
    protected void SetAuthorizationHeader(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Authorization header'ı temizler
    /// </summary>
    protected void ClearAuthorizationHeader()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Yeni kullanıcı kaydı yapar ve user ID döndürür
    /// </summary>
    protected async System.Threading.Tasks.Task<Guid> RegisterUserAndGetIdAsync(string email, string password, string userName)
    {
        var registerRequest = new RegisterCommand(userName, email, password);
        var registerResponse = await Client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterCommandResponse>();
        return registerResult!.UserId;
    }

    /// <summary>
    /// Proje üyesi ekler (Owner/Member/Viewer)
    /// </summary>
    protected async System.Threading.Tasks.Task AddProjectMemberAsync(Guid projectId, Guid userId, ProjectRole role)
    {
        var projectMember = ProjectMember.Create(projectId, (UserId)userId, role);

        DbContext.Set<ProjectMember>().Add(projectMember);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Proje, modül, use case, task hiyerarşisini oluşturur ve tüm ID'leri döndürür
    /// </summary>
    protected async System.Threading.Tasks.Task<TaskTestContext> CreateFullTaskContextAsync()
    {
        // Owner kullanıcısı oluştur ve token al
        var ownerGuid = Guid.NewGuid().ToString("N");
        var ownerEmail = $"owner{ownerGuid}@test.com";
        var ownerUsername = $"owner_{ownerGuid}";
        var ownerId = await RegisterUserAndGetIdAsync(ownerEmail, "Test123!@#", ownerUsername);
        var ownerToken = await LoginAndGetTokenAsync(ownerEmail, "Test123!@#");
        SetAuthorizationHeader(ownerToken);

        // Proje oluştur (owner otomatik Owner rolüne sahip olur)
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        // Modül oluştur
        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        // Use case oluştur
        var createUseCaseRequest = new { Title = "Test UseCase", Description = "Test Description", ImportantNotes = (string?)null };
        var useCaseResponse = await Client.PostAsJsonAsync($"/api/v1/usecases/modules/{module!.Id}", createUseCaseRequest);
        useCaseResponse.EnsureSuccessStatusCode();
        var useCase = await useCaseResponse.Content.ReadFromJsonAsync<CreateUseCaseCommandResponse>();

        // Task oluştur
        var createTaskRequest = new { Title = "Test Task", TaskType = TaskType.Feature, Description = "Test Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null };
        var taskResponse = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{useCase!.Id}", createTaskRequest);
        taskResponse.EnsureSuccessStatusCode();
        var task = await taskResponse.Content.ReadFromJsonAsync<CreateTaskCommandResponse>();

        // Member kullanıcısı oluştur ve projeye ekle
        var memberGuid = Guid.NewGuid().ToString("N");
        var memberEmail = $"member{memberGuid}@test.com";
        var memberUsername = $"member_{memberGuid}";
        var memberId = await RegisterUserAndGetIdAsync(memberEmail, "Test123!@#", memberUsername);
        var memberToken = await LoginAndGetTokenAsync(memberEmail, "Test123!@#");
        await AddProjectMemberAsync(project.Id, memberId, ProjectRole.Member);

        // Viewer kullanıcısı oluştur ve projeye ekle
        var viewerGuid = Guid.NewGuid().ToString("N");
        var viewerEmail = $"viewer{viewerGuid}@test.com";
        var viewerUsername = $"viewer_{viewerGuid}";
        var viewerId = await RegisterUserAndGetIdAsync(viewerEmail, "Test123!@#", viewerUsername);
        var viewerToken = await LoginAndGetTokenAsync(viewerEmail, "Test123!@#");
        await AddProjectMemberAsync(project.Id, viewerId, ProjectRole.Viewer);

        // Non-member kullanıcısı oluştur (projeye eklenmez)
        var nonMemberGuid = Guid.NewGuid().ToString("N");
        var nonMemberEmail = $"nonmember{nonMemberGuid}@test.com";
        var nonMemberUsername = $"nonmember_{nonMemberGuid}";
        var nonMemberId = await RegisterUserAndGetIdAsync(nonMemberEmail, "Test123!@#", nonMemberUsername);
        var nonMemberToken = await LoginAndGetTokenAsync(nonMemberEmail, "Test123!@#");

        return new TaskTestContext(
            project,
            module,
            useCase,
            task,
            ownerToken,
            ownerId,
            memberToken,
            memberId,
            viewerToken,
            viewerId,
            nonMemberToken,
            nonMemberId
        );
    }

    /// <summary>
    /// Task atama durumunu kontrol eder
    /// </summary>
    protected async System.Threading.Tasks.Task<bool> IsTaskAssignedToUserAsync(Guid taskId, Guid userId)
    {
        var response = await Client.GetAsync($"/api/v1/tasks/{taskId}");
        if (!response.IsSuccessStatusCode)
            return false;

        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        return task?.AssigneeId == userId;
    }

    /// <summary>
    /// Task durumunu kontrol eder
    /// </summary>
    protected async System.Threading.Tasks.Task<TaskState> GetTaskStateAsync(Guid taskId)
    {
        var response = await Client.GetAsync($"/api/v1/tasks/{taskId}");
        response.EnsureSuccessStatusCode();

        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        return task!.TaskState;
    }

    public void Dispose()
    {
        ServiceScope?.Dispose();
        Client?.Dispose();
        DbContext?.Dispose();
    }
}

/// <summary>
/// Test senaryolarında kullanılan task context bilgileri
/// </summary>
public record TaskTestContext(
    CreateProjectCommandResponse Project,
    CreateModuleCommandResponse Module,
    CreateUseCaseCommandResponse UseCase,
    CreateTaskCommandResponse Task,
    string OwnerToken,
    Guid OwnerId,
    string MemberToken,
    Guid MemberId,
    string ViewerToken,
    Guid ViewerId,
    string NonMemberToken,
    Guid NonMemberId
);

