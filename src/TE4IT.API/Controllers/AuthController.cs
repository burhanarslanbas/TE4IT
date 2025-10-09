using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TE4IT.Application.Features.Auth.Commands.Login;
using TE4IT.Application.Features.Auth.Commands.Refresh;
using TE4IT.Application.Features.Auth.Commands.Register;
using TE4IT.Application.Features.Auth.Commands.Revoke;

namespace TE4IT.API.Controllers;

/// <summary>
/// Kimlik doğrulama endpoint'leri
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Yeni kullanıcı kaydı oluşturur
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterCommand(request.Email, request.Password), ct);
        if (result is null) return BadRequest();
        return Created($"/api/v1/users/{result.UserId}", result);
    }

    /// <summary>
    /// Kullanıcı girişi yapar
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new LoginCommand(request.Email, request.Password), ct);
        if (result is null) return Unauthorized();
        return Ok(result);
    }

    /// <summary>
    /// Access token'ı yeniler
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("fixed-refresh")]
    [ProducesResponseType(typeof(RefreshTokenCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var response = await mediator.Send(new RefreshTokenCommand(request.RefreshToken), ct);
        return Ok(response);
    }

    /// <summary>
    /// Refresh token'ı iptal eder
    /// </summary>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ok = await mediator.Send(new RevokeRefreshTokenCommand(request.RefreshToken, "user requested", ip), ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}

/// <summary>
/// Kayıt request DTO
/// </summary>
public sealed record RegisterRequest(string Email, string Password);

/// <summary>
/// Giriş request DTO
/// </summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>
/// Token yenileme request DTO
/// </summary>
public sealed record RefreshRequest(string RefreshToken);

