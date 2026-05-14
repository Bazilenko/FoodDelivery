using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Catalog.Bll.DTOs.Dish;

namespace Catalog.Api.Controllers
{
    /// <summary>
    /// Single dish detail with modifier groups — opened by clicking a dish card.
    /// </summary>
    [ApiController]
    [Route("api/dishes")]
    [Produces("application/json")]
    public class DishesPublicController : ControllerBase
    {
        private readonly IDishPublicService _service;

        public DishesPublicController(IDishPublicService service) => _service = service;

        /// <summary>Full dish detail including modifier groups and options.</summary>
        [HttpGet("{dishId:int}")]
        [ProducesResponseType(typeof(DishDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishDetailDto>> GetDetail(int dishId, CancellationToken ct)
        {
            var result = await _service.GetDetailAsync(dishId, ct);
            return Ok(result);
        }
    }
}