using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Catalog.Bll.DTOs.Restaurant;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Api.Controllers
{
    /// <summary>
    /// Restaurant owner manages their own profile.
    /// All endpoints require the "RestaurantOwner" role.
    /// </summary>
    [ApiController]
    [Route("catalog/owner/restaurant")]
    [Authorize(Roles = "RestaurantOwner")]
    [Produces("application/json")]
    public class RestaurantOwnerController : ControllerBase
    {
        private readonly IRestaurantOwnerService _service;

        public RestaurantOwnerController(IRestaurantOwnerService service) => _service = service;

        /// <summary>Register a new restaurant (first-time onboarding).</summary>
        [HttpPost]
        [ProducesResponseType(typeof(RestaurantProfileDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RestaurantProfileDto>> Create(
            [FromBody] RestaurantCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetProfile), null, created);
        }

        /// <summary>Get the current restaurant's full profile.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(RestaurantProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RestaurantProfileDto>> GetProfile(CancellationToken ct)
        {
            var result = await _service.GetProfileAsync(ct);
            return Ok(result);
        }

        /// <summary>Update name, description, image, and delivery radius.</summary>
        [HttpPut]
        [ProducesResponseType(typeof(RestaurantProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RestaurantProfileDto>> UpdateProfile(
            [FromBody] RestaurantUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateProfileAsync(dto, ct);
            return Ok(updated);
        }

        /// <summary>Replace the restaurant's full cuisine list.</summary>
        [HttpPut("cuisines")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCuisines(
            [FromBody] RestaurantCuisinesUpdateDto dto, CancellationToken ct)
        {
            await _service.UpdateCuisinesAsync(dto, ct);
            return NoContent();
        }
    }
}