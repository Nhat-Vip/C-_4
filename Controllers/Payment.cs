using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        return View(seats);
    }
    [HttpPost]
    public async Task<IActionResult> Index(List<ShowTimeSeatDto> seatDtos)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        await Task.Delay(2000);

        var random = new Random();
        bool isSuccess = random.Next(0, 2) == 1;

        if (isSuccess)
        {
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
            foreach (var item in seatDtos)
            {
                var TicketDetail = new TicketDetail();
                TicketDetail.SeatId = item.SeatId;
                TicketDetail.ShowTimeId = item.ShowTimeId;
                TicketDetail.TicketId = ticket.TicketId;
                _context.TicketDetails.Add(TicketDetail);
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

}