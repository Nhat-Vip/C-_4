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
        if (status == "ALL")
        {
            events = await _context.Events.Where(s => s.UserId == int.Parse(userId!)).ToListAsync();
        }
        else
        {
            events = await _context.Events.Where(s => s.UserId == int.Parse(userId!) && s.EventStatus.ToString() == status).ToListAsync();
        }
        return Json(new { Events = events });
    }

    [HttpPost]
    public async Task<IActionResult> GetTickets(int page, int limit, [FromQuery] string status)
    {
        var loadTickets = new List<Ticket>();
        if (status == "All")
        {
            loadTickets = await _context.Tickets.Skip((page - 1) * limit).Take(limit).Include(t => t.TicketDetails).Include(s => s.Event).ToListAsync();
            Console.WriteLine("AAAAAAAAA");
        }
        else
        {
            loadTickets = await _context.Tickets.Skip((page - 1) * limit).Take(limit).Where(tk => tk.Status.ToString() == status).Include(t => t.TicketDetails).Include(s => s.Event).ToListAsync();
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBb");

        }
        if (loadTickets == null || !loadTickets.Any())
        {
            return Json(new { tickets = new List<Ticket>(), message = "Không có vé nào" });
        }
        var ticketData = loadTickets.Select(t => new
        {
            eventName = t.Event.EventName, // Giả định có trường EventName, thay bằng tên thực tế
            date = t.Event.StartEvent.ToString("dd/MM/yyyy"), // Giả định có trường EventDate
            location = t.Event.EventAddress, // Giả định có trường Location
            status = t.Status.ToString(),
            quantity = t.TicketDetails.Count(), 
            total = t.Total.ToString("C0",new CultureInfo("vi-VN"))
        }).ToList();
        return Json(new { tickets = loadTickets });
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