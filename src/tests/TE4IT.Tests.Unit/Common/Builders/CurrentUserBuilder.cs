using Moq;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Domain.Constants;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Tests.Unit.Common.Builders;

/// <summary>
/// Fluent API ile ICurrentUser mock oluşturma builder'ı
/// </summary>
public class CurrentUserBuilder
{
    private UserId? _id = new UserId(Guid.NewGuid());
    private bool _isAuthenticated = true;
    private List<string> _roles = new();

    public CurrentUserBuilder WithId(UserId id)
    {
        _id = id;
        return this;
    }

    public CurrentUserBuilder WithId(Guid id)
    {
        _id = new UserId(id);
        return this;
    }

    public CurrentUserBuilder WithNoId()
    {
        _id = null;
        return this;
    }

    public CurrentUserBuilder WithIsAuthenticated(bool isAuthenticated)
    {
        _isAuthenticated = isAuthenticated;
        return this;
    }

    public CurrentUserBuilder WithRole(string role)
    {
        if (!_roles.Contains(role))
        {
            _roles.Add(role);
        }
        return this;
    }

    public CurrentUserBuilder WithRoles(params string[] roles)
    {
        _roles.AddRange(roles);
        return this;
    }

    public CurrentUserBuilder AsTrial()
    {
        _roles.Clear();
        _roles.Add(RoleNames.Trial);
        return this;
    }

    public CurrentUserBuilder AsAdministrator()
    {
        _roles.Clear();
        _roles.Add(RoleNames.Administrator);
        return this;
    }

    public CurrentUserBuilder AsTeamLead()
    {
        _roles.Clear();
        _roles.Add(RoleNames.TeamLead);
        return this;
    }

    public ICurrentUser Build()
    {
        var mock = new Mock<ICurrentUser>();
        mock.Setup(x => x.Id).Returns(_id);
        mock.Setup(x => x.IsAuthenticated).Returns(_isAuthenticated);
        mock.Setup(x => x.Roles).Returns(_roles);
        mock.Setup(x => x.IsInRole(It.IsAny<string>())).Returns<string>(role => _roles.Contains(role));
        return mock.Object;
    }

    public static CurrentUserBuilder Create() => new();
}

