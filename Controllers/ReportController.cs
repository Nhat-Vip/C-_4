using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ReportController:Controller
{

    private readonly MyDbContext _context;
    public ReportController(MyDbContext context)
    {
        _context = context;
    }

    // Báo cáo tổng doanh thu theo sự kiện
    public async Task<IActionResult> EventRevenue()
    {
        var report = await _context.Events
            .Where(e=> e.EventStatus == EventStatus.Approved
            && e.UserId == Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            .Select(e => new
            {
                EventId = e.EventId,
                EventName = e.EventName,
                TotalRevenue = e.Tickets
                    .Where(t => t.Status == TicketStatus.Success)
                    .Sum(t => t.Total),
                TotalTicketsSold = e.Tickets
                    .Where(t => t.Status == TicketStatus.Success)
                    .SelectMany(t => t.TicketDetails)
                    .Count()
            })
            .OrderByDescending(e => e.TotalRevenue)
            .ToListAsync();

        return View(report);
    }
    // API trả về chi tiết doanh thu 1 sự kiện
    [HttpGet]
    public async Task<IActionResult> EventRevenueDetail([FromQuery]int eventId)
    {

        var eventDetail = await _context.ShowTimes
            .Where(st => st.EventId == eventId)
            .Include(st => st.ShowTimeTicketGroups)
                .ThenInclude(g => g.SeatGroup)
                    .ThenInclude(sg => sg.Seats)
            .Include(st => st.ShowTimeSeats)
                .ThenInclude(ss => ss.TicketDetail)
            .Select(st => new
            {
                ShowTimeId = st.Id,
                ShowTime = st.StartTime,
                Groups = st.ShowTimeTicketGroups.Select(g => new
                {
                    g.Name,
                    g.Price,
                    TicketsSold = g.SeatGroup.Seats
                        .SelectMany(s => st.ShowTimeSeats.Where(ss => ss.SeatId == s.SeatId))
                        .Where(ss => ss.TicketDetail != null)
                        .Count()
                })
            })
            .ToListAsync();

        return Json(eventDetail);
    }
}
