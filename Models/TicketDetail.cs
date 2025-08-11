public class TicketDetail
{
    public int TicketDetailId { set; get; }
    public int? TicketId { set; get; }
    public Ticket Ticket { set; get; } = null!;
    public int? ShowTimeId { set; get; }
    public int SeatId{ set; get; }
    public ShowTimeSeat ShowTimeSeat { set; get; } = null!;
}