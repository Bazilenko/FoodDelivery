using Auth.DTOs.Requests;
using Auth.DTOs.Responses;
using Auth.Entities;
using Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Auth.Context;
using Microsoft.EntityFrameworkCore;


namespace Auth.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtService;
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _db;

    // Refresh token cookie settings
    private const string RefreshTokenCookie = "refreshToken";
    private readonly CookieOptions _cookieOptions = new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddDays(7)
    };

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtService,
        IEmailService emailService,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _emailService = emailService;
        _db = db;
    }

    // ── POST /auth/register ───────────────────────────────────────────────────

    [HttpPost("register")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest req, CancellationToken ct)
    {
        if (await _userManager.FindByEmailAsync(req.Email) is not null)
            return Conflict(new MessageResponse("Email is already registered."));

        var user = new ApplicationUser
        {
            UserName = req.Username,
            Email = req.Email,
            FirstName = req.FirstName,
            LastName = req.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        // Assign default "User" role
        await _userManager.AddToRoleAsync(user, "User");

        // Send verification email
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailVerificationAsync(user.Email!, user.Id.ToString(), token, ct);

        return StatusCode(StatusCodes.Status201Created,
            new MessageResponse("Registration successful. Please verify your email."));
    }

    // ── POST /auth/login ──────────────────────────────────────────────────────

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
    [FromBody] LoginRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, req.Password))
            return Unauthorized(new MessageResponse("Invalid email or password."));

        //if (!user.EmailConfirmed)
        //    return Unauthorized(new MessageResponse("Please verify your email before logging in."));

        if (await _userManager.IsLockedOutAsync(user))
            return Unauthorized(new MessageResponse("Account is temporarily locked. Try again later."));

        var roles = await _userManager.GetRolesAsync(user);

        int? restaurantId = null;

        if (roles.Contains("RestaurantOwner"))
        {

            var foundId = await _db.UserRestaurant
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RestaurantId)
                .FirstOrDefaultAsync(ct);

            if (foundId != 0)
            {
                restaurantId = foundId;
            }
        }
        // ─────────────────────────────────────────────────────────────────────


        var accessToken = _jwtService.GenerateAccessToken(user, roles, restaurantId);
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        Response.Cookies.Append(RefreshTokenCookie, refreshToken.Token, _cookieOptions);

        return Ok(BuildAuthResponse(accessToken, user, roles));
    }

    // ── POST /auth/refresh-token ──────────────────────────────────────────────

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> RefreshToken(CancellationToken ct)
    {
        // Token comes from cookie, not body — more secure
        var tokenValue = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(tokenValue))
            return Unauthorized(new MessageResponse("Refresh token not found."));

        var refreshToken = await _jwtService.GetRefreshTokenAsync(tokenValue, ct);

        if (refreshToken is null || !refreshToken.IsActive)
            return Unauthorized(new MessageResponse("Invalid or expired refresh token."));

        var user = refreshToken.User;
        var roles = await _userManager.GetRolesAsync(user);

        // Token rotation — revoke old, issue new
        var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, ct);
        await _jwtService.RevokeRefreshTokenAsync(
            refreshToken, "Rotated", newRefreshToken.Token, ct);

        var accessToken = _jwtService.GenerateAccessToken(user, roles);

        Response.Cookies.Append(RefreshTokenCookie, newRefreshToken.Token, _cookieOptions);

        return Ok(BuildAuthResponse(accessToken, user, roles));
    }

    // ── POST /auth/revoke-token ───────────────────────────────────────────────

    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeToken(CancellationToken ct)
    {
        var tokenValue = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(tokenValue))
            return BadRequest(new MessageResponse("No refresh token provided."));

        var refreshToken = await _jwtService.GetRefreshTokenAsync(tokenValue, ct);
        if (refreshToken is null || !refreshToken.IsActive)
            return BadRequest(new MessageResponse("Invalid or already revoked token."));

        await _jwtService.RevokeRefreshTokenAsync(refreshToken, "Revoked by user", ct: ct);

        return NoContent();
    }

    // ── POST /auth/logout ─────────────────────────────────────────────────────

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var tokenValue = Request.Cookies[RefreshTokenCookie];

        if (!string.IsNullOrEmpty(tokenValue))
        {
            var refreshToken = await _jwtService.GetRefreshTokenAsync(tokenValue, ct);
            if (refreshToken?.IsActive == true)
                await _jwtService.RevokeRefreshTokenAsync(refreshToken, "Logged out", ct: ct);
        }

        // Clear the cookie
        Response.Cookies.Delete(RefreshTokenCookie);

        return NoContent();
    }

    // ── POST /auth/verify-email ───────────────────────────────────────────────

    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest req)
    {
        var user = await _userManager.FindByIdAsync(req.UserId);
        if (user is null)
            return BadRequest(new MessageResponse("Invalid verification link."));

        var result = await _userManager.ConfirmEmailAsync(user, req.Token);
        if (!result.Succeeded)
            return BadRequest(new MessageResponse("Email verification failed. The link may have expired."));

        return Ok(new MessageResponse("Email verified successfully. You can now log in."));
    }

    // ── POST /auth/resend-verification ───────────────────────────────────────

    [HttpPost("resend-verification")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendVerification(
        [FromBody] ResendVerificationRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);

        // Always return 200 to prevent email enumeration
        if (user is null || user.EmailConfirmed)
            return Ok(new MessageResponse("If that email exists and is unverified, a new link has been sent."));

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailVerificationAsync(user.Email!, user.Id.ToString(), token, ct);

        return Ok(new MessageResponse("If that email exists and is unverified, a new link has been sent."));
    }

    // ── POST /auth/forgot-password ────────────────────────────────────────────

    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);

        // Always return 200 to prevent email enumeration
        if (user is null || !user.EmailConfirmed)
            return Ok(new MessageResponse("If that email is registered, a reset link has been sent."));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendPasswordResetAsync(user.Email!, user.Id.ToString(), token, ct);

        return Ok(new MessageResponse("If that email is registered, a reset link has been sent."));
    }

    // ── POST /auth/reset-password ─────────────────────────────────────────────

    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(req.UserId);
        if (user is null)
            return BadRequest(new MessageResponse("Invalid reset link."));

        var result = await _userManager.ResetPasswordAsync(user, req.Token, req.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        // Security: revoke all active refresh tokens after password reset
        await _jwtService.RevokeAllUserRefreshTokensAsync(user.Id, "Password reset", ct);

        return Ok(new MessageResponse("Password reset successful. Please log in with your new password."));
    }

    // ── POST /auth/change-password ────────────────────────────────────────────

    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        var userId = _userManager.GetUserId(User)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Unauthorized();

        var result = await _userManager.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        // Security: revoke all other active refresh tokens after password change
        await _jwtService.RevokeAllUserRefreshTokensAsync(user.Id, "Password changed", ct);

        return Ok(new MessageResponse("Password changed successfully. Please log in again."));
    }

    [HttpPost("select-restaurant")]
    [Authorize(Roles = "RestaurantOwner")]
    public async Task<ActionResult<AuthResponse>> SelectRestaurant(
    [FromBody] SelectRestaurantRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var ownsRestaurant = await _db.UserRestaurant
            .AnyAsync(ur => ur.UserId == userId && ur.RestaurantId == req.RestaurantId, ct);

        if (!ownsRestaurant)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new MessageResponse("Ви не маєте прав доступу до цього ресторану."));
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        var newAccessToken = _jwtService.GenerateAccessToken(user, roles, req.RestaurantId);
        var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        Response.Cookies.Append(RefreshTokenCookie, newRefreshToken.Token, _cookieOptions);

        return Ok(BuildAuthResponse(newAccessToken, user, roles));
    }

    [HttpGet("my-restaurants")]
    [Authorize(Roles = "RestaurantOwner")]
    public async Task<ActionResult<IEnumerable<UserRestaurantDto>>> GetMyRestaurants(CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var restaurants = await _db.UserRestaurant
            .Where(ur => ur.UserId == userId)
            .Select(ur => new UserRestaurantDto(ur.RestaurantId))
            .ToListAsync(ct);

        return Ok(restaurants);
    }

    private static AuthResponse BuildAuthResponse(
        string accessToken, ApplicationUser user, IList<string> roles)
        => new(
            AccessToken: accessToken,
            TokenType: "Bearer",
            ExpiresIn: 15 * 60,   // 15 minutes in seconds
            User: new UserDto(
                user.Id,
                user.UserName!,
                user.Email!,
                user.FirstName,
                user.LastName,
                user.EmailConfirmed,
                user.CreatedAt,
                roles));
}