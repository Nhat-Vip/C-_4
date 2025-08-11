using System.ComponentModel.DataAnnotations;

public class TicketGroup
{
    public int TicketGroupId { set; get; }
    [Required]
    public string? Name { set; get; }
    public int MaxTicket { set; get; }
    public List<SeatGroup> SeatGroups { set; get; } = new();
    public List<ShowTimeTicketGroup> ShowTimeTicketGroups { set; get; } = new();
}