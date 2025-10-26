using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Persistence.Common.Identity;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Exceptions.Auth;

namespace TE4IT.Infrastructure.Auth.Services;

public sealed class UserAccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : IUserAccountService
{
    public async Task<Guid> RegisterAsync(string userName, string email, string password, CancellationToken ct)
    {
        // Username ve email benzersizlik kontrolü
        var existingUserByUserName = await userManager.FindByNameAsync(userName);
        if (existingUserByUserName != null)
        {
            throw new DuplicateUserNameException(userName);
        }

        var existingUserByEmail = await userManager.FindByEmailAsync(email);
        if (existingUserByEmail != null)
        {
            throw new DuplicateEmailException(email);
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new UserRegistrationFailedException(errors);
        }

        // Yeni kullanıcıya varsayılan rol ata (Trial kullanıcısı)
        var roleResult = await userManager.AddToRoleAsync(user, RoleNames.Trial);
        if (!roleResult.Succeeded)
        {
            // Rol atama başarısız olursa kullanıcıyı sil
            await userManager.DeleteAsync(user);
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            throw new UserRegistrationFailedException($"Rol atama başarısız: {errors}");
        }

        return user.Id;
    }

    public async Task<Guid> ValidateCredentialsAsync(string email, string password, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new InvalidCredentialsException(email);
        }

        var check = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!check.Succeeded)
        {
            throw new InvalidCredentialsException(email);
        }

        return user.Id;
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return token;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return false;

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return false;

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }
}

