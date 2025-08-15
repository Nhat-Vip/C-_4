using System.ComponentModel.DataAnnotations;

public class User
{
    public int UserId { set; get; }
    [Required]
    public string? UserName { set; get; }
    public string? Avatar { set; get; }
    [Required]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { set; get; }
    public Role Role { set; get; }
    [MaxLength(11)]
    [Phone]
    public string? PhoneNumber { set; get; }
    public string? PassWord { set; get; }
    public List<Ticket> Tickets { set; get; } = new();
    public List<Event> Events { set; get; } = new();
}
public enum Role
{
    Admin,
    User
}