using Microsoft.AspNetCore.Mvc;

public class EventController : Controller
{
    [HttpPost]
    public IActionResult BuyTicket(int EventId, string CustomerName, int Quantity)
    {

        TempData["Message"] = $"Đặt {Quantity} vé cho sự kiện {EventId} thành công!";
        return RedirectToAction("Index", "Home");
    }
}
