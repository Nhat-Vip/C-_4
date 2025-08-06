public class TicketDetail
{
    public int TicketDetailId { set; get; }
    public int? TicketId { set; get; }
    public Ticket Ticket { set; get; } = null!;
    public int? SeatId { set; get; }
    public Seat Seat { set; get; } = null!;
}