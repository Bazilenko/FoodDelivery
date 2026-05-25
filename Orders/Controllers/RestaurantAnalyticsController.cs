using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Bll.DTOs.Analytics;
using Orders.Shared.DTOs;
using Orders.Bll.Services.Interfaces;
 
namespace Orders.Api.Controllers;
[ApiController]
[Route("orders/analytics")]
//[Authorize(Roles = "RestaurantOwner")]
[Produces("application/json")]
public class RestaurantAnalyticsController : ControllerBase
{
    private readonly IRestaurantAnalyticsService _service;
 
    public RestaurantAnalyticsController(IRestaurantAnalyticsService service)
        => _service = service;
 
    /// <summary>
    /// Full dashboard — revenue summary, order summary, top dishes,
    /// top categories, payments, retention and period comparison.
    /// All queries run in parallel server-side.
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DashboardDto>> GetDashboard(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetDashboardAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    // ── Revenue ───────────────────────────────────────────────────────────────
 
    [HttpGet("revenue/summary")]
    [ProducesResponseType(typeof(RevenueSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RevenueSummaryDto>> GetRevenueSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetRevenueSummaryAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("revenue/by-day")]
    [ProducesResponseType(typeof(IEnumerable<RevenuePeriodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RevenuePeriodDto>>> GetRevenueByDay(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetRevenueByDayAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("revenue/by-week")]
    [ProducesResponseType(typeof(IEnumerable<RevenuePeriodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RevenuePeriodDto>>> GetRevenueByWeek(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetRevenueByWeekAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("revenue/by-month")]
    [ProducesResponseType(typeof(IEnumerable<RevenuePeriodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RevenuePeriodDto>>> GetRevenueByMonth(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetRevenueByMonthAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    // ── Orders ────────────────────────────────────────────────────────────────
 
    [HttpGet("orders/summary")]
    [ProducesResponseType(typeof(OrderSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderSummaryDto>> GetOrderSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetOrderSummaryAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("orders/peak-hours")]
    [ProducesResponseType(typeof(IEnumerable<PeakHourDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PeakHourDto>>> GetPeakHours(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPeakHoursAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    // ── Dishes ────────────────────────────────────────────────────────────────
 
    [HttpGet("dishes/top-by-quantity")]
    [ProducesResponseType(typeof(IEnumerable<TopDishDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopDishDto>>> GetTopDishesByQuantity(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetTopDishesByQuantityAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("dishes/top-by-revenue")]
    [ProducesResponseType(typeof(IEnumerable<TopDishDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopDishDto>>> GetTopDishesByRevenue(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetTopDishesByRevenueAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("categories/top")]
    [ProducesResponseType(typeof(IEnumerable<TopCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopCategoryDto>>> GetTopCategories(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetTopCategoriesAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("options/top")]
    [ProducesResponseType(typeof(IEnumerable<TopOptionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopOptionDto>>> GetTopOptions(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetTopOptionsAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    // ── Customers ─────────────────────────────────────────────────────────────
 
    [HttpGet("customers/top")]
    [ProducesResponseType(typeof(IEnumerable<TopCustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopCustomerDto>>> GetTopCustomers(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetTopCustomersAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("customers/retention")]
    [ProducesResponseType(typeof(CustomerRetentionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CustomerRetentionDto>> GetCustomerRetention(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetCustomerRetentionAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    // ── Payments ──────────────────────────────────────────────────────────────
 
    [HttpGet("payments/summary")]
    [ProducesResponseType(typeof(PaymentSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentSummaryDto>> GetPaymentSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPaymentSummaryAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
}