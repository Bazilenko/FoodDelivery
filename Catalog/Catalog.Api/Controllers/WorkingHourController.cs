using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Catalog.Bll.DTOs.WorkingHour;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("catalog/owner/working-hours")]
    [Authorize(Roles = "RestaurantOwner")]
    [Produces("application/json")]
    public class WorkingHoursController : ControllerBase
    {
        private readonly IWorkingHourService _service;

        public WorkingHoursController(IWorkingHourService service) => _service = service;

        /// <summary>Full week schedule (all 7 days).</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WorkingHourDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WorkingHourDto>>> GetWeekSchedule(CancellationToken ct)
            => Ok(await _service.GetWeekScheduleAsync(ct));

        /// <summary>Schedule for a specific day (0 = Sunday … 6 = Saturday).</summary>
        [HttpGet("day/{dayOfWeek:int}")]
        [ProducesResponseType(typeof(WorkingHourDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkingHourDto>> GetByDay(int dayOfWeek, CancellationToken ct)
            => Ok(await _service.GetByDayAsync(dayOfWeek, ct));

        [HttpPost]
        [ProducesResponseType(typeof(WorkingHourDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkingHourDto>> Create(
            [FromBody] WorkingHourCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetByDay), new { dayOfWeek = created.DayOfWeek }, created);
        }

        [HttpPut]
        [ProducesResponseType(typeof(WorkingHourDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<WorkingHourDto>>> Update( [FromBody] WorkingHoursUpdateDto dto, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}