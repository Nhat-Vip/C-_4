using System.ComponentModel.DataAnnotations;

public class SeatGroup
{
    public int SeatGroupId { set; get; }
    [Required]
    public string? Name { set; get; }
    public string? Color{ set; get; }
    public string? Rotate { set; get; }
    public int PosX { set; get; }
    public int PosY { set; get; }
    public int Cols { set; get; }
    public int Rows { set; get; }
    public List<Seat> Seats { set; get; } = new();
    public int? SeatingChartId{ set; get; }
    public SeatingChart? SeatingChart { set; get; }
    public List<ShowTimeTicketGroup>? ShowTimeTicketGroups { set; get; }
}