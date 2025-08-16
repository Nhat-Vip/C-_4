using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserController : Controller
{
    private readonly IUserService _user;
    private readonly MyDbContext _context;
    public UserController(IUserService user,MyDbContext context)
    {
        _context = context;
        _user = user;
    }
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _user.GetUserById(int.Parse(userId!));
        return View(user);
    }
    public async Task<IActionResult> UpdateProfile(User us, IFormFile UserImg)
    {
        if (UserImg != null && UserImg?.Length > 0)
        {
            us.Avatar = await SaveImg(UserImg);
        }
        var user = await _user.UpdateUser(us);

        if (user == false)
        {
            TempData["Error"] = "Co Loi Xay ra";
        }
        else
        {
            TempData["Success"] = "Cập nhật thông tin tài khoản thành công";
        }
        return RedirectToAction("Index");


    }

    public async Task<IActionResult> GetEvent([FromQuery] string status)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var events = new List<Event>();
        if (status == "All")
        {
            events = await _context.Events.Where(s => s.UserId == int.Parse(userId!) && s.EventStatus != EventStatus.Draft).ToListAsync();
        }
        else
        {
            events = await _context.Events.Where(s => s.UserId == int.Parse(userId!) && s.EventStatus.ToString() == status).ToListAsync();
        }
        return Json(new { Events = events });
    }

    public async Task<IActionResult> GetTickets([FromQuery] int page,[FromQuery] int limit, [FromQuery] string status)
    {
        var loadTickets = new List<Ticket>();
        Console.WriteLine("Page" + (page - 1));
        Console.WriteLine("Limit" + limit);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (status == "All")
        {
            loadTickets = await _context.Tickets.Where(t => t.UserId.ToString() == userId)
                                                .Include(t => t.TicketDetails).Include(s => s.Event)
                                                .Skip((page - 1) * limit).ToListAsync();
        }
        else
        {
            loadTickets = await _context.Tickets.Where(tk => tk.Status.ToString() == status && tk.UserId.ToString() == userId)
                                                .Include(t => t.TicketDetails).Include(s => s.Event)
                                                .Skip((page - 1) * limit).Take(limit).ToListAsync();

        }
        if (loadTickets == null || !loadTickets.Any())
        {
            return Json(new { tickets = new List<Ticket>(), message = "Không có vé nào" });
        }
        var ticketData = loadTickets.Select(t => new
        {
            eventId = t.EventId,
            ticketId = t.TicketId,
            eventName = t.Event?.EventName ?? "Chưa có dữ liệu",
            date = t.Event?.StartEvent.ToString("dd/MM/yyyy") ?? "Chưa có dữ liệu",
            location = t.Event?.EventAddress ?? "Chưa có dữ liệu",
            status = t.Status.ToString(),
            quantity = t.TicketDetails?.Count() ?? 0,
            total = t.Total.ToString("C0", new CultureInfo("vi-VN")),
            ticketDetails = t.TicketDetails?.Select(td => new
            {
                ticketDetailId = td.TicketDetailId,
                seatId = td.SeatId,
                showTimeId = td.ShowTimeId
            }).ToList()
        }).DistinctBy(e=> e.eventId).ToList();
        Console.WriteLine("Số lượng vé: " + ticketData.Count);
        return Json(new { tickets = ticketData });
    }


    [HttpGet]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        try
        {
            // Tìm ticket cần xóa
            var ticket = await _context.Tickets
                .Include(t => t.TicketDetails)
                .Include(t => t.Payment)
                .Include(t => t.Refund)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound(new { message = "Không tìm thấy vé để xóa" });
            }

            // Xóa các TicketDetails liên quan
            _context.TicketDetails.RemoveRange(ticket.TicketDetails);

            // Xóa Payment nếu tồn tại
            if (ticket.Payment != null)
            {
                _context.Payments.Remove(ticket.Payment);
            }

            // Xóa Refund nếu tồn tại
            if (ticket.Refund != null)
            {
                _context.Refunds.Remove(ticket.Refund);
            }

            // Xóa ticket
            _context.Tickets.Remove(ticket);

            // Lưu thay đổi vào DB
            await _context.SaveChangesAsync();

            return Json(new { success = "Xóa vé thành công" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi xóa vé: " + ex.Message });
        }
    }

    public async Task<string> SaveImg(IFormFile file)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/Images/{fileName}";
    }
}