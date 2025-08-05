using System.ComponentModel.DataAnnotations;

public class SeatingChart
{
    public int SeatingChartId { set; get; }
    [Required]
    public string? Name { set; get; }
    // public int? UserId{ set; get; }
    // public User? User { set; get; }
    public List<Event> Events { set; get; } = new();
    public List<SeatGroup> SeatGroups { set; get; } = new();
}