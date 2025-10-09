using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Domain.Constants;
using TE4IT.Persistence.Relational.Identity;

namespace TE4IT.Infrastructure.Auth.Services;

public sealed class UserAccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : IUserAccountService
{
    public async Task<Guid?> RegisterAsync(string email, string password, CancellationToken ct)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true
        };
        
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded) return null;

        // Yeni kullanıcıya varsayılan rol ata
        var roleResult = await userManager.AddToRoleAsync(user, RoleNames.Employee);
        if (!roleResult.Succeeded)
        {
            // Rol atama başarısız olursa kullanıcıyı sil
            await userManager.DeleteAsync(user);
            return null;
        }

        return user.Id;
    }

    public async Task<Guid?> ValidateCredentialsAsync(string email, string password, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return null;
        var check = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        return check.Succeeded ? user.Id : null;
    }
}

