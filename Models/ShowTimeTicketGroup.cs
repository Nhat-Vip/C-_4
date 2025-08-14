public class ShowTimeTicketGroup
{
    public int ShowTimeId { set; get; }
    public ShowTime ShowTime { set; get; } = null!;
    public int SeatGroupId { get; set; }
    public SeatGroup SeatGroup { get; set; } = null!;
    public decimal Price { set; get; }
    public string? Name { set; get; }
    public int MaxTicket { set; get; }
    public DateTime TicketSaleStart { set; get; }
    public DateTime TicketSaleEnd { set; get; }
}