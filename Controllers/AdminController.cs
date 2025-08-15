using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _admin;
    private readonly MyDbContext _context;
    public AdminController(IAdminService admin, MyDbContext context)
    {
        _admin = admin;
        _context = context;
    }
    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult UserManagement()
    {
        return View();
    }

    public IActionResult EventManagement()
    {
        return View();
    }

    [HttpGet("GetUsers")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var users = await _context.Users
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        var userData = users.Select(u => new
        {
            userId = u.UserId,
            userName = u.UserName,
            email = u.Email,
            phoneNumber = u.PhoneNumber,
            role = u.Role.ToString(),
            avatar = u.Avatar ?? "No avatar"
        }).ToList();

        return Json(new { users = userData, total = await _context.Users.CountAsync() });
    }

    [HttpGet("GetEvents")]
    public async Task<IActionResult> GetEvents([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var events = await _context.Events
            .Where(e=> e.EventStatus  == EventStatus.Pending)
            .Include(e => e.User)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        var eventData = events.Select(e => new
        {
            eventId = e.EventId,
            eventName = e.EventName,
            eventAddress = e.EventAddress,
            startEvent = e.StartEvent.ToString("dd/MM/yyyy"),
            endDateTime = e.EndDateTime.ToString("dd/MM/yyyy"),
            eventType = e.EventType.ToString(),
            eventStatus = e.EventStatus.ToString(),
            userName = e.User?.UserName ?? "Chưa có",
            image = e.Image ?? "No image"
        }).ToList();

        return Json(new { events = eventData, total = await _context.Events.CountAsync() });
    }

    [HttpPost("SaveUserWithFile")]
    public async Task<IActionResult> SaveUserWithFile([FromForm] User user, IFormFile? AvatarFile)
    {
        if (!TryValidateModel(user))
        {
            return BadRequest(new { message = "Dữ liệu không hợp lệ" });
        }

        try
        {
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}_{AvatarFile.FileName}";
                var filePath = Path.Combine("wwwroot/Images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(stream);
                }

                user.Avatar = "/Images/" + fileName;
            }

            if (user.UserId == 0)
            {
                _context.Users.Add(user);
            }
            else
            {
                var existingUser = await _context.Users.FindAsync(user.UserId);
                if (existingUser == null)
                    return NotFound(new { message = "Không tìm thấy người dùng" });

                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.PassWord = user.PassWord ?? existingUser.PassWord;
                existingUser.Role = user.Role;
                if (!string.IsNullOrEmpty(user.Avatar))
                    existingUser.Avatar = user.Avatar ?? existingUser.Avatar;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu thành công", userId = user.UserId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lưu: " + ex.Message });
        }
    }

    [HttpDelete("DeleteUser/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }
            if(user.UserId.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return BadRequest(new { message = "Không thể xóa chính mình" });
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Xóa thành công" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi xóa: " + ex.Message });
        }
    }

    [HttpPost("UpdateEventStatus")]
    public async Task<IActionResult> UpdateEventStatus([FromBody] UpdateEventStatusRequest data)
    {
        try
        {
            var ev = await _context.Events.FindAsync(data.EventId);
            if (ev == null)
            {
                return NotFound(new { message = "Không tìm thấy sự kiện" });
            }

            if (Enum.TryParse<EventStatus>(data.Status, true, out var newStatus))
            {
                ev.EventStatus = newStatus;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Cập nhật trạng thái thành {ev.EventStatus.GetVietNameseLabel()} thành công" });
            }
            return BadRequest(new { message = "Trạng thái không hợp lệ" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi cập nhật: " + ex.Message });
        }
    }
}