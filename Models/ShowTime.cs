public class ShowTime
{
    public int Id { set; get; }
    public DateTime StartTime { set; get; }
    public DateTime EndTime { set; get; }
    public int EventId { set; get; }
    public Event? Event { set; get; } = null!;
    public List<ShowTimeTicketGroup> ShowTimeTicketGroups { set; get; } = new List<ShowTimeTicketGroup>();
    public List<ShowTimeSeat> ShowTimeSeats { set; get; } = new List<ShowTimeSeat>();
}