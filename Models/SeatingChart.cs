using System.ComponentModel.DataAnnotations;

public class SeatingChart
{
    public int SeatingChartId { set; get; }
    [Required]
    public string? Name { set; get; }
    public int? PosX{ set; get; }
    public int? PosY{ set; get; }
    public Event Event { set; get; } = null!;
    public List<SeatGroup> SeatGroups { set; get; } = new();
}