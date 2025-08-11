public class ShowTimeTicketGroup
{
    public int ShowTimeId { set; get; }
    public ShowTime ShowTime { set; get; } = null!;
    public int TicketGruopId { set; get; }
    public TicketGroup TicketGroup { set; get; } = null!;
    public decimal Price { set; get; }
    public int Quantity { set; get; }
    public DateTime TicketSaleStart { set; get; }
    public DateTime TicketSaleEnd { set; get; }

}