using System.ComponentModel.DataAnnotations;

public class TicketGroup
{
    public int TicketGroupId { set; get; }
    [Required]
    public string? Name { set; get; }
    public decimal Price { set; get; }
    public int Quantity { set; get; }
    public DateTime TicketSaleStart { set; get; }
    public DateTime TicketSaleEnd { set; get; }
    public List<SeatGroup> SeatGroups { set; get; } = new();
}