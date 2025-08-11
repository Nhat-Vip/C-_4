using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class EventController : Controller
{
    // private readonly Dictionary<string, decimal> TicketPrices = new Dictionary<string, decimal>
    // {
    //     { "Người Về Giấc Mơ", 3200000 },
    //     { "Trái Người Về Tự Do", 2700000 },
    //     { "Kỳ Vọng Sai Lầm", 2500000 },
    //     { "Đừng Chờ Anh Nữa", 2200000 },
    //     { "Mơ", 2000000 },
    //     { "Mộng", 1700000 },
    //     { "Êm", 1200000 },
    //     { "Thơ", 800000 }
    // };

    // [HttpGet]
    // public IActionResult SelectSeats(int eventId)
    // {
    //     ViewBag.EventId = eventId;
    //     ViewBag.Prices = TicketPrices;
    //     return View();
    // }

    // [HttpPost]
    // public IActionResult BuyTicket(int EventId, string CustomerName, string SelectedSeats, string TicketType)
    // {
    //     if (string.IsNullOrEmpty(SelectedSeats) || string.IsNullOrEmpty(TicketType))
    //     {
    //         TempData["Message"] = "Vui lòng chọn ghế và loại vé!";
    //         return RedirectToAction("SelectSeats", new { eventId = EventId });
    //     }

    //     decimal ticketPrice = TicketPrices.ContainsKey(TicketType) ? TicketPrices[TicketType] : 0;
    //     var seats = SelectedSeats.Split(',');
    //     int quantity = seats.Length;
    //     decimal totalPrice = ticketPrice * quantity;

    //     TempData["Message"] = $"✅ Đặt {quantity} vé ({TicketType}) thành công!<br>" +
    //                           $"🎟 Ghế: {string.Join(", ", seats)}<br>" +
    //                           $"💰 Tổng tiền: {totalPrice:N0} VNĐ";

    //     return RedirectToAction("Index", "Home");
    // }
    private readonly MyDbContext _context;
    private readonly IEventService _event;
    public EventController(MyDbContext context, IEventService @event)
    {
        _context = context;
        _event = @event;
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Details()
    {
        return View();
        // var ev = await _event.GetById(id);
        // if (ev != null)
        // {
        //     return View(ev);
        // }
        // TempData["Error"] = "Không tìm thấy event";
        // return RedirectToAction("Index", "Home");
    }
    public IActionResult BookingTicket()
    {
        return View("Booking");
    }
}
