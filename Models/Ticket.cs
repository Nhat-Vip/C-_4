using System.ComponentModel.DataAnnotations;

public class Ticket
{
    public int TicketId { set; get; }
    [Required]
    public string? TicketType { set; get; }
    public decimal Price { set; get; }
    public DateTime CreateAt { set; get; } = DateTime.Now;
    public TicketStatus Status { set; get; }
    public int? SeatId{ set; get; }
    public Seat Seat { set; get; } = null!;
    public int? EventId{ set; get; }
    public Event Event { set; get; } = null!;
    public int? UserId{ set; get; }
    public User User { set; get; } = null!;
    public Payment Payment { set; get; } = null!;
}
public enum TicketStatus
{
    Success,
    Processing,
    Cancel

}