using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Commands = TE4IT.Application.Features.Auth.Commands;

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
    [ProducesResponseType(typeof(Commands.Register.RegisterCommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] Commands.Register.RegisterCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Created($"/api/v1/users/{result!.UserId}", result);
    }

    /// <summary>
    /// Kullanıcı girişi yapar
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Commands.Login.LoginCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] Commands.Login.LoginCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Access token'ı yeniler
    /// </summary>
    [HttpPost("refreshToken")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Commands.RefreshToken.RefreshTokenCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] Commands.RefreshToken.RefreshTokenCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        return Ok(response);
    }

    /// <summary>
    /// Refresh token'ı iptal eder
    /// </summary>
    [HttpPost("revokeRefreshToken")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] Commands.RevokeRefreshToken.RevokeRefreshTokenCommand command, CancellationToken ct)
    {
        var ok = await mediator.Send(command, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Şifre sıfırlama linki gönderir (kullanıcı var/yok detayını sızdırmaz)
    /// </summary>
    [HttpPost("forgotPassword")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Commands.ForgotPassword.ForgotPasswordCommandResponse), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> ForgotPassword([FromBody] Commands.ForgotPassword.ForgotPasswordCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Accepted(result);
    }

    /// <summary>
    /// Token ile şifreyi sıfırlar
    /// </summary>
    [HttpPost("resetPassword")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Commands.ResetPassword.ResetPasswordCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] Commands.ResetPassword.ResetPasswordCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.Success)
            return BadRequest(result.Message);
        return Ok(result);
    }

    /// <summary>
    /// Uygulama içi şifre değiştirme (authenticated kullanıcılar için)
    /// </summary>
    [HttpPost("changePassword")]
    [Authorize]
    [ProducesResponseType(typeof(Commands.ChangePassword.ChangePasswordCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] Commands.ChangePassword.ChangePasswordCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.Success)
            return BadRequest(result.Message);
        return Ok(result);
    }
}
