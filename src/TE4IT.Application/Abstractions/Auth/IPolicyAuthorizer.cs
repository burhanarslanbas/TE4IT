namespace TE4IT.Application.Abstractions.Auth;

public interface IPolicyAuthorizer
{
    Task<bool> AuthorizeAsync(string policyName, CancellationToken ct);
}


