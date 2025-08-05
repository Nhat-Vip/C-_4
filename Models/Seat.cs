public class Seat
{
    public int SeatId { set; get; }
    public string? SeatName { set; get; }
    public bool Status { set; get; } = false;
    public int PosX { set; get; }
    public int PosY { set; get; }

    public int SeatGroupId{ set; get; }
    public SeatGroup SeatGroup { set; get; } = null!;
    public Ticket Ticket { set; get; } = null!;
}