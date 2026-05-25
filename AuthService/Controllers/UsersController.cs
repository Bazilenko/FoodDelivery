using Auth.Entities;
using Auth.DTOs.Requests;
using Auth.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Auth.Context;

namespace Auth.Controllers;

[ApiController]
[Route("users")]
[Authorize]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    [HttpPost("internal/user-restaurant")]
    [AllowAnonymous] 
    public async Task<IActionResult> LinkUserToRestaurantInternal([FromBody] LinkUserRestaurantDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user is null) return NotFound("User not found");

        var alreadyExists = await _db.UserRestaurant
            .AnyAsync(ur => ur.UserId == dto.UserId && ur.RestaurantId == dto.RestaurantId);

        if (!alreadyExists)
        {
            _db.UserRestaurant.Add(new UserRestaurant
            {
                UserId = dto.UserId,
                RestaurantId = dto.RestaurantId
            });
            await _db.SaveChangesAsync();
        }

        return Task.FromResult<IActionResult>(NoContent()).Result;
    }

    // ── GET /users ────────────────────────────────────────────────────────────

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken ct)
    {
        var users = await _userManager.Users.ToListAsync(ct);

        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(MapToDto(user, roles));
        }

        return Ok(result);
    }

    // ── GET /users/{id} ───────────────────────────────────────────────────────

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        if (!IsOwnerOrAdmin(id))
            return Forbid();

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(MapToDto(user, roles));
    }

    // ── PUT /users/{id} ───────────────────────────────────────────────────────

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateProfile(
        int id, [FromBody] UpdateProfileRequest req)
    {
        if (!IsOwnerOrAdmin(id))
            return Forbid();

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return NotFound();

        if (req.FirstName is not null) user.FirstName = req.FirstName;
        if (req.LastName is not null) user.LastName = req.LastName;
        if (req.Username is not null) user.UserName = req.Username;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(MapToDto(user, roles));
    }

    // ── DELETE /users/{id} ────────────────────────────────────────────────────

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return NotFound();

        // Soft delete — keeps audit trail
        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    // ── GET /users/{id}/refresh-tokens ───────────────────────────────────────

    [HttpGet("{id:int}/refresh-tokens")]
    [ProducesResponseType(typeof(IEnumerable<RefreshTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RefreshTokenDto>>> GetRefreshTokens(
        int id, CancellationToken ct)
    {
        if (!IsOwnerOrAdmin(id))
            return Forbid();

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return NotFound();

        var tokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == id)
            .OrderByDescending(rt => rt.CreatedAt)
            .Select(rt => new RefreshTokenDto(
                rt.Id,
                rt.Token,
                rt.CreatedAt,
                rt.ExpiresAt,
                rt.RevokedAt,
                rt.IsActive))
            .ToListAsync(ct);

        return Ok(tokens);
    }

    // ── GET /users/{id}/roles ─────────────────────────────────────────────────

    [HttpGet("{id:int}/roles")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<string>>> GetRoles(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }

    // ── POST /users/{id}/roles ────────────────────────────────────────────────

    [HttpPost("{id:int}/roles")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleRequest req)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return NotFound();

        if (await _userManager.IsInRoleAsync(user, req.Role))
            return BadRequest(new { message = $"User already has role '{req.Role}'." });

        var result = await _userManager.AddToRoleAsync(user, req.Role);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return NoContent();
    }

    // ── DELETE /users/{id}/roles/{role} ───────────────────────────────────────

    [HttpDelete("{id:int}/roles/{role}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRole(int id, string role)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return NotFound();

        if (!await _userManager.IsInRoleAsync(user, role))
            return BadRequest(new { message = $"User does not have role '{role}'." });

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return NoContent();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Returns true if the current user is the owner of the resource or an Admin.</summary>
    private bool IsOwnerOrAdmin(int resourceUserId)
    {
        var currentUserId = _userManager.GetUserId(User);
        var isAdmin = User.IsInRole("Admin");
        return isAdmin || currentUserId == resourceUserId.ToString();
    }

    private static UserDto MapToDto(ApplicationUser user, IList<string> roles)
        => new(
            user.Id,
            user.UserName!,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.EmailConfirmed,
            user.CreatedAt,
            roles);
}