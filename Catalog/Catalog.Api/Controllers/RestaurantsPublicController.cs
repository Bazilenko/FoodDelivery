using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Catalog.Bll.DTOs.Restaurant;
using Catalog.Bll.DTOs.Dish;

namespace Catalog.Api.Controllers
{
    /// <summary>
    /// Public restaurant profile and menu — no authentication required.
    /// </summary>
    [ApiController]
    [Route("catalog/restaurants")]
    [Produces("application/json")]
    public class RestaurantsPublicController : ControllerBase
    {
        private readonly IRestaurantPublicService _service;

        public RestaurantsPublicController(IRestaurantPublicService service) => _service = service;

        /// <summary>Public restaurant profile: addresses, contacts, working hours, cuisines.</summary>
        [HttpGet("{restaurantId:int}")]
        [ProducesResponseType(typeof(RestaurantDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RestaurantDetailDto>> GetDetail(
            int restaurantId, CancellationToken ct)
        {
            var result = await _service.GetDetailAsync(restaurantId, ct);
            return Ok(result);
        }

        /// <summary>Full restaurant menu: categories with their available dishes.</summary>
        [HttpGet("{restaurantId:int}/menu")]
        [ProducesResponseType(typeof(IEnumerable<CategoryWithDishesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryWithDishesDto>>> GetMenu(
            int restaurantId, CancellationToken ct)
        {
            var result = await _service.GetMenuAsync(restaurantId, ct);
            return Ok(result);
        }
    }
}