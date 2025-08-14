using System.ComponentModel.DataAnnotations;

public class Ticket
{
    public int TicketId { set; get; }
    public decimal Total { set; get; }
    public DateTime CreateAt { set; get; } = DateTime.Now;
    public TicketStatus Status { set; get; }
    public int? EventId { set; get; }
    public Event Event { set; get; } = null!;
    public int? UserId { set; get; }
    public User User { set; get; } = null!;
    public Payment? Payment { set; get; }
    public Refund? Refund { set; get; }
    public List<TicketDetail> TicketDetails { set; get; } = new();
}
public enum TicketStatus
{
    Success,
    Processing,
    Cancel

}