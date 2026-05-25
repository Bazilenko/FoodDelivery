namespace Catalog.Bll.DTOs.ModifierGroup
{
    public class ModifierGroupUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MinSelect { get; set; }
        public int MaxSelect { get; set; }
    }
}
