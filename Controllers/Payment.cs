using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class PaymentController : Controller
{
    private readonly MyDbContext _context;
    public PaymentController(MyDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var seatsJson = TempData["SeatsJson"]?.ToString();
#pragma warning disable CS8604 // Possible null reference argument.
        var seats = JsonConvert.DeserializeObject<List<ShowTimeSeatDto>>(seatsJson);
#pragma warning restore CS8604 // Possible null reference argument.
        var ev = _context.Events
            .FirstOrDefault(e => e.EventId == Convert.ToInt32(TempData["EventId"]));
        ViewBag.BankName = ev?.BankName;
        ViewBag.BankNumber = ev?.BankNumber;
        TempData.Keep("SeatsJson");
        TempData.Keep("EventId");
        return View(seats);
    }
    [HttpPost]
    public async Task<IActionResult> Index(List<ShowTimeSeatDto> seatDtos)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        await Task.Delay(2000);

        var random = new Random();
        bool isSuccess = random.Next(0, 2) == 1; // Giả lập thành công hoặc thất bại ngẫu nhiên

        if (isSuccess)
        {
            Console.WriteLine("Luu ticket hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");
            var ticket = new Ticket();
            ticket.Total = seatDtos.Sum(s => s.Price);
            ticket.CreateAt = DateTime.Now;
            ticket.Status = TicketStatus.Success;
            ticket.EventId = Convert.ToInt32(TempData["EventId"]);
            ticket.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            var payment = new Payment();
            payment.Amount = ticket.Total;
            payment.CreateAt = DateTime.Now;
            payment.Status = PaymentStatus.Success;
            payment.TicketId = ticket.TicketId;
            _context.Payments.Add(payment);
            Console.WriteLine("Số lượng seatDtos: " + seatDtos.Count);
            foreach (var item in seatDtos)
            {
                Console.WriteLine("Dang Luu ticket detail");
                Console.WriteLine("SeatId" + item.SeatId);
                Console.WriteLine("ShowtimeId" + item.ShowTimeId);
                Console.WriteLine("Dang Luu ticket detail");

                var ticketDetail = new TicketDetail();
                ticketDetail.SeatId = item.SeatId;
                ticketDetail.ShowTimeId = item.ShowTimeId;
                ticketDetail.TicketId = ticket.TicketId;
                ticketDetail.Ticket = ticket;
                _context.TicketDetails.Add(ticketDetail);
            }
            await _context.SaveChangesAsync();
            // Thanh toán thành công → commit
            await transaction.CommitAsync();
            // TempData["Success"] = "Thanh toan thanh cong";
            return RedirectToAction("PaymentResult", new { success = true });
        }
        else
        {
            // Thanh toán thất bại → rollback
            await transaction.RollbackAsync();
            Console.WriteLine("Da goiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii2");

            var seatIds = seatDtos?.Select(s => s.SeatId).ToList();
            var seatEntities = await _context.ShowTimeSeats
            .Where(s => seatIds!.Contains(s.SeatId))
            .ToListAsync();

            foreach (var seat in seatEntities)
            {
                seat.IsBooked = false;
            }

            await _context.SaveChangesAsync();

            // TempData["Error"] = "Thanh toan that bai";
            return RedirectToAction("PaymentResult", new { success = false });
        }
    }
    public IActionResult PaymentResult(bool success)
    {
        if (success)
            TempData["Message"] = "Thanh toán thành công";
        else
            TempData["Message"] = "Thanh toán thất bại";

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> CancelBooking([FromBody] List<ShowTimeSeatDto> seatDtos)
    {
        Console.WriteLine("Da goiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii");
        var seatIds = seatDtos?.Select(s => s.SeatId).ToList();
        var seatEntities = await _context.ShowTimeSeats
        .Where(s => seatIds!.Contains(s.SeatId))
        .ToListAsync();

        foreach (var seat in seatEntities)
        {
            seat.IsBooked = false;
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

}