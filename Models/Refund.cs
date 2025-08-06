public class Refund
{
    public int RefundId { set; get; }
    public decimal Amount { set; get; }
    public RefundStatus Status { set; get; } = RefundStatus.Pending;
    public DateTime CreateAt{ set; get; } = DateTime.Now;
    public string? Reason { set; get; }
    public int? TicketId { set; get; }
    public Ticket Ticket { set; get; } = null!;

}
public enum RefundStatus
{
    Pending,
    Completed,
    Rejected
}
public static class EnumExtensions
{
    public static string GetVietNameseLabel(this RefundStatus status)
    {
        return status switch
        {
            RefundStatus.Pending => "Chờ xác nhận",
            RefundStatus.Completed => "Thành công",
            RefundStatus.Rejected => "Từ chối",
            _ => "Không biết"
        };
    }
}