using System.ComponentModel.DataAnnotations;

public class User
{
    public int UserId { set; get; }
    [Required]
    public string? UserName { set; get; }
    [Required]
    public string? Email { set; get; }
    public Role Role { set; get; }
    [MaxLength(11)]
    public string? PhoneNumber { set; get; }
    [Required]
    public string? PassWord { set; get; }
    public List<Ticket> Tickets { set; get; } = new();
    public List<SeatingChart> SeatingCharts { set; get; } = new();
    public List<Event> Events { set; get; } = new();
}
public enum Role
{
    Admin,
    User
}