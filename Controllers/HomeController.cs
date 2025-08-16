using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASM_C_4.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ASM_C_4.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEventService _event;
    private readonly MyDbContext _context;

    public HomeController(ILogger<HomeController> logger, IEventService @event,MyDbContext context)
    {
        _context = context;
        _event = @event;
        _logger = logger;

    }

    public async Task<IActionResult> Index()
    {
        var query = TempData["SearchQuery"]?.ToString();
        var events = new List<Event>();
        if (!string.IsNullOrEmpty(query))
        {
            events = await _context.Events
                .Where(e => e.EventName.Contains(query) && e.EventStatus == EventStatus.Approved)
                .Include(s => s.ShowTimes)
                .ThenInclude(s => s.ShowTimeTicketGroups)
                .ToListAsync();
            return View(events);
        }
        events = await _context.Events.Where(e=>e.EventStatus == EventStatus.Approved).Include(s => s.ShowTimes).ThenInclude(s => s.ShowTimeTicketGroups).ToListAsync();
        return View(events);
    }

    public async Task<IActionResult> GetEvent(string type)
    {
        var ev = new List<Event>();
        if (type == "All")
        {
            ev = await _context.Events.Where(e=>e.EventStatus == EventStatus.Approved).Include(s => s.ShowTimes).ThenInclude(s => s.ShowTimeTicketGroups).ToListAsync();
        }
        else
        {
            ev = await _context.Events.Where(e => e.EventType.ToString() == type && e.EventStatus == EventStatus.Approved).Include(s => s.ShowTimes).ThenInclude(s => s.ShowTimeTicketGroups).ToListAsync();
        }
        return PartialView("_EventPartial", ev);
    }
    public async Task<IActionResult> Search(string query)
    {
        // if (string.IsNullOrEmpty(query))
        // {
        //     return RedirectToAction("Index");
        // }

        // var events = await _context.Events
        //     .Where(e => e.EventName.Contains(query) && e.EventStatus == EventStatus.Approved)
        //     .Include(s => s.ShowTimes)
        //     .ThenInclude(s => s.ShowTimeTicketGroups)
        //     .ToListAsync();
        TempData["SearchQuery"] = query;

        return RedirectToAction("Index");
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
