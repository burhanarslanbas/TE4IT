using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Exceptions.Auth;
using TE4IT.Persistence.Relational.Identity;

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

        // Yeni kullanıcıya varsayılan rol ata
        var roleResult = await userManager.AddToRoleAsync(user, RoleNames.Employee);
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
}

