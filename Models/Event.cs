using System.ComponentModel.DataAnnotations;

public class Event
{
    public int EventId { set; get; }
    [Required]
    public string? EventName { set; get; }
    public string? EventAddress { set; get; }
    public string? Image { set; get; }
    public string? SubImage { set; get; }
    public DateTime CreateAt { set; get; } = DateTime.Now;
    public string? Description { set; get; }
    public DateTime StartEvent { set; get; }
    public DateTime EndDateTime { set; get; }
    public EventType EventType { set; get; }
    public EventStatus EventStatus { set; get; }
    public int? SeatingChartId { set; get; }
    public string? SoNganHang { set; get; }
    public SeatingChart SeatingChart { set; get; } = null!;
    public int? UserId { set; get; }
    public User? User { set; get; }
    public List<Ticket> Tickets { set; get; } = new();
    public List<ShowTime> ShowTimes { set; get; } = new();

}
public enum EventType
{
    Concert,
    Sport
}
public enum EventStatus
{
    Pending,
    Approved,
    Past,
    Upcoming,
    Cancel

}
public static class EnumExtension
{
    public static string GetVietNameseLabel(this EventStatus eventStatus)
    {
        return eventStatus switch
        {
            EventStatus.Pending => "Đang chờ xử lý",
            EventStatus.Approved => "Đã duyệt",
            EventStatus.Past => "Đã qua",
            EventStatus.Upcoming => "Sắp tới",
            EventStatus.Cancel => "Bị từ chối",
            _ => "Khong biet"
        };
    }
}