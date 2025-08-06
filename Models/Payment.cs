public class Payment
{
    public int PaymentId { set; get; }
    public decimal Amount { set; get; }
    public PaymentStatus Status { set; get; }
    public DateTime CreateAt { set; get; } = DateTime.Now;
    public int TicketId{ set; get; }
    public Ticket Ticket { set; get; } = null!;
}

public enum PaymentStatus
{
    Pending,
    Success,
    Failed,
    Refunded
}