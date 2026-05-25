
namespace Orders.Bll.DTOs.OrderDish;
public class DishSnapshotResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public List<OptionSnapshotResponseDto> Options { get; set; } = [];
}