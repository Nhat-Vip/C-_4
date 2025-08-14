using System.ComponentModel.DataAnnotations;

public class SeatingChart
{
    public int SeatingChartId { set; get; }
    public int? PosX{ set; get; }
    public int? PosY{ set; get; }

    public int? EventId{ set; get; }
    public Event? Event { set; get; }
    public List<SeatGroup> SeatGroups { set; get; } = new();
}