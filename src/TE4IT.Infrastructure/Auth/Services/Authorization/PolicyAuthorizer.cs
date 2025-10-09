using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Infrastructure.Auth.Services.Authorization;

public sealed class PolicyAuthorizer(IAuthorizationService authorizationService, IHttpContextAccessor accessor) : IPolicyAuthorizer
{
    public async Task<bool> AuthorizeAsync(string policyName, CancellationToken ct)
    {
        var principal = accessor.HttpContext?.User;
        if (principal is null) return false;
        var result = await authorizationService.AuthorizeAsync(principal, null, policyName);
        return result.Succeeded;
    }
}

