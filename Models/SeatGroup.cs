using System.ComponentModel.DataAnnotations;

public class SeatGroup
{
    public int SeatGroupId { set; get; }
    [Required]
    public string? Name { set; get; }
    public int PosX { set; get; }
    public int PosY { set; get; }
    public List<Seat> Seats { set; get; } = new();
    public int? SeatingChartId{ set; get; }
    public SeatingChart SeatingChart { set; get; } = null!;
    public int? TicketGroupId{ set; get; }
    public TicketGroup? TicketGroup { set; get; }
}