using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Catalog.Bll.DTOs.Dish;

namespace Catalog.Api.Controllers
{
    /// <summary>
    /// Single dish detail with modifier groups — opened by clicking a dish card.
    /// </summary>
    [ApiController]
    [Route("catalog/dishes")]
    [Produces("application/json")]
    public class DishesPublicController : ControllerBase
    {
        private readonly IDishPublicService _service;
        private readonly IRestaurantMenuService _menuService;

        public DishesPublicController(IDishPublicService service, IRestaurantMenuService menuService)
        {
            _menuService = menuService;
            _service = service;
        }

        [HttpGet("{dishId:int}")]
        [ProducesResponseType(typeof(DishDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishDetailDto>> GetDishDetail(
        int dishId,
        CancellationToken ct)
        {
            var result = await _menuService.GetDishDetailAsync(dishId, ct);
            return Ok(result);
        }

    }
}