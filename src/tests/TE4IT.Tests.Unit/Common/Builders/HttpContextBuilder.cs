using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TE4IT.Tests.Unit.Common.Builders;

/// <summary>
/// Fluent API ile HttpContext oluşturma builder'ı
/// </summary>
public class HttpContextBuilder
{
    private ClaimsPrincipal? _user;
    private string? _ipAddress;
    private string? _requestPath;
    private string? _method;

    public HttpContextBuilder WithUser(ClaimsPrincipal user)
    {
        _user = user;
        return this;
    }

    public HttpContextBuilder WithIpAddress(string ipAddress)
    {
        _ipAddress = ipAddress;
        return this;
    }

    public HttpContextBuilder WithRequestPath(string path)
    {
        _requestPath = path;
        return this;
    }

    public HttpContextBuilder WithMethod(string method)
    {
        _method = method;
        return this;
    }

    public HttpContext Build()
    {
        var context = new DefaultHttpContext();
        
        if (_user != null)
        {
            context.User = _user;
        }

        if (_ipAddress != null)
        {
            context.Connection.RemoteIpAddress = IPAddress.Parse(_ipAddress);
        }

        if (_requestPath != null)
        {
            context.Request.Path = _requestPath;
        }

        if (_method != null)
        {
            context.Request.Method = _method;
        }

        return context;
    }

    public static HttpContextBuilder Create() => new();
}
