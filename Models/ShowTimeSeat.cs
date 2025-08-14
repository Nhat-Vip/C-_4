public class ShowTimeSeat
{
    public int ShowTimeId { set; get; }
    public ShowTime ShowTime { set; get; } = null!;

    public int SeatId { set; get; }
    public Seat Seat { set; get; } = null!;

    public bool IsBooked { set; get; }
    public TicketDetail? TicketDetail { set; get; } = null!;
}