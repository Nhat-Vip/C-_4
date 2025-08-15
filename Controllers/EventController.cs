using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
    private readonly ISeatingChartService _stc;
    public EventController(MyDbContext context, IEventService @event, ISeatingChartService stc)
    {
        _stc = stc;
        _context = context;
        _event = @event;
    }
    [Authorize]
    [Route("Event/Create/{eventId?}")]
    public IActionResult Create(int? eventId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.UserId = userId;
        var ev = new Event();
        if (eventId.HasValue)
        {
            ev = _context.Events.Include(e => e.SeatingChart)
                                 .ThenInclude(s => s!.SeatGroups)
                                    .ThenInclude(s => s.Seats)
                                .Include(e => e.ShowTimes)
                                    .ThenInclude(s => s.ShowTimeTicketGroups)
                                .FirstOrDefault(e => e.EventId == eventId.Value);
            if (ev == null)
            {
                return NotFound();
            }
        }
        return View(ev);
    }

    [HttpPost]
    public async Task<IActionResult> SaveEventInfo([FromForm] Event ev,[FromForm] IFormFile ImageFile)
    {
        try
        {
            // var e = await _event.GetById(ev.EventId);
            if (ev.EventId > 0)
            {
                // e.EventName = ev.EventName;
                // e.Description = ev.Description;
                // e.StartEvent = ev.StartEvent;
                // e.EndDateTime = ev.EndDateTime;
                // e.EventType = ev.EventType;
                // e.EventAddress = ev.EventAddress;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    ev.Image = await SaveImg(ImageFile);
                }
                _context.Events.Update(ev);
                await _context.SaveChangesAsync();
                Console.WriteLine("Cập nhật sự kiện thành công");
                return Ok(new { eventId = ev.EventId });
            }

            if (ev == null)
            {
                return BadRequest("Dữ liệu gửi lên không hợp lệ");
            }

            ev.EventStatus = EventStatus.Draft;
            ev.CreateAt = DateTime.Now;
            if (ImageFile == null || ImageFile.Length == 0)
            {
                return BadRequest("Vui lòng chọn ảnh.");
            }

            ev.Image = await SaveImg(ImageFile);

            var result = await _event.Create(ev);
            if (!result)
            {
                return BadRequest("Co loi xay ra");
            }
            else
            {
                return Ok(new { eventId = ev.EventId });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }

    }
    [HttpPost]
    public async Task<IActionResult> SaveSeatingChart([FromBody] SeatingChart stc, [FromQuery] int eventId)
    {
        try
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
        if (ev == null)
        {
            return NotFound("Sự kiện không tồn tại");
        }

        // Kiểm tra xem SeatingChart đã tồn tại với EventId hay chưa
        var seatingChart = await _context.SeatingCharts
            .Include(sc => sc.SeatGroups)
            .ThenInclude(sg => sg.Seats)
            .FirstOrDefaultAsync(sc => sc.EventId == eventId);

        if (seatingChart != null)
        {
            // // Cập nhật các thuộc tính của SeatingChart
            // seatingChart.PosX = stc.PosX;
            // seatingChart.PosY = stc.PosY;

            // // Xóa các SeatGroup cũ (bao gồm Seats liên quan)
            // foreach (var oldSeatGroup in seatingChart.SeatGroups)
            // {
            //     _context.Seats.RemoveRange(oldSeatGroup.Seats); // Xóa Seats của SeatGroup
            // }
            // _context.SeatGroups.RemoveRange(seatingChart.SeatGroups); // Xóa SeatGroups

            // // Thêm các SeatGroup mới từ stc
            // seatingChart.SeatGroups = stc.SeatGroups ?? new List<SeatGroup>();
            // foreach (var seatGroup in seatingChart.SeatGroups)
            // {
            //     seatGroup.SeatingChartId = seatingChart.SeatingChartId; // Gán khóa ngoại
            //     foreach (var seat in seatGroup.Seats)
            //     {
            //         seat.SeatGroupId = seatGroup.SeatGroupId; // Gán khóa ngoại
            //     }
            // }

            // Cập nhật SeatingChart
            _context.SeatingCharts.Update(seatingChart);

            foreach (var seatGroup in stc.SeatGroups)
            {
                // Kiểm tra xem SeatGroup đã tồn tại trong SeatingChart hay chưa
                var existingSeatGroup = seatingChart.SeatGroups.FirstOrDefault(sg => sg.SeatGroupId == seatGroup.SeatGroupId);
                if (existingSeatGroup != null)
                {
                    // Cập nhật SeatGroup hiện tại
                    existingSeatGroup.Name = seatGroup.Name;
                    existingSeatGroup.PosX = seatGroup.PosX;
                    existingSeatGroup.PosY = seatGroup.PosY;

                    // Cập nhật Seats trong SeatGroup
                    foreach (var seat in seatGroup.Seats)
                    {
                        var existingSeat = existingSeatGroup.Seats.FirstOrDefault(s => s.SeatId == seat.SeatId);
                        if (existingSeat != null)
                        {
                            // existingSeat.SeatName = seat.SeatName;
                            existingSeat.SeatGroup = seatGroup;
                        }
                        else
                        {
                            // Thêm Seat mới nếu không tồn tại
                            existingSeatGroup.Seats.Add(seat);
                        }
                    }
                }
                else
                {
                    // Thêm SeatGroup mới nếu không tồn tại
                    seatingChart.SeatGroups.Add(seatGroup);
                }
            }

            await _context.SaveChangesAsync();

            // Lấy danh sách SeatGroup để trả về dropdown
            var seatGroups = await _context.SeatGroups
                .Where(s => s.SeatingChartId == seatingChart.SeatingChartId)
                .ToListAsync();

            if (!seatGroups.Any())
            {
                Console.WriteLine("Không tìm thấy SeatGroup nào.");
            }
            else
            {
                foreach (var sg in seatGroups)
                {
                    Console.WriteLine($"ID: {sg.SeatGroupId}, Name: {sg.Name}");
                }
            }

            ViewBag.SeatGroup = new SelectList(seatGroups, "SeatGroupId", "Name");
            return PartialView("_SeatGroupDropdown");
        }

            var even = await _context.Events.FirstOrDefaultAsync(e=>e.EventId == eventId);
            if (even == null)
            {
                return NotFound();
            }
            var result = await _stc.Create(stc);
            if (!result)
            {
                return BadRequest("Co Loi xay raaaaa");
            }
            else
            {
                var seatGroup = await _context.SeatGroups.Where(s => s.SeatingChartId == stc.SeatingChartId).ToListAsync();
                Console.WriteLine("SeatGroup", seatGroup);
                foreach (var sg in seatGroup)
                {
                    Console.WriteLine($"ID: {sg.SeatGroupId}, Name: {sg.Name}");
                }
                ViewBag.SeatGroup = new SelectList(seatGroup, "SeatGroupId", "Name");
                return PartialView("_SeatGroupDropdown");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveShowTime([FromBody] List<ShowTime> showTimes)
    {
        try
        {
            if (showTimes == null || !showTimes.Any())
                return BadRequest("Dữ liệu gửi lên không hợp lệ");

            var eventEntity = await _context.ShowTimes
                            .Where(st => st.EventId == showTimes.First().EventId)
                            .Include(st => st.ShowTimeSeats)
                            .Include(st => st.ShowTimeTicketGroups)
                            .ToListAsync();

            if (eventEntity.Any())
            {
                // Xóa các ShowTime cũ và các liên kết với ShowTimeSeats và ShowTimeTicketGroups
                Console.WriteLine("Xóa các ShowTime cũ và các liên kết với ShowTimeSeats và ShowTimeTicketGroups");
                var seatsToDelete = eventEntity.SelectMany(s => s.ShowTimeSeats).ToList();
                var groupsToDelete = eventEntity.SelectMany(s => s.ShowTimeTicketGroups).ToList();

                _context.ShowTimeSeats.RemoveRange(seatsToDelete);
                _context.ShowTimeTicketGroups.RemoveRange(groupsToDelete);
                _context.ShowTimes.RemoveRange(eventEntity);

                await _context.SaveChangesAsync();
            }


            foreach (var st in showTimes)
            {
                Console.WriteLine($"ShowTime FE: Id={st.Id}, EventId={st.EventId}, Start={st.StartTime}, End={st.EndTime}, TicketGroupCount={st.ShowTimeTicketGroups?.Count}");
                ShowTime showTime;

                // INSERT mới

                showTime = new ShowTime
                {
                    EventId = st.EventId,
                    StartTime = st.StartTime,
                    EndTime = st.EndTime
                };

                _context.ShowTimes.Add(showTime);
                await _context.SaveChangesAsync(); // Lưu để có Id cho ShowTime



                foreach (var tg in st.ShowTimeTicketGroups)
                {
                    Console.WriteLine($"ShowTimeTicketGroup FE: Id={tg.SeatGroupId}, ShowTimeId={showTime.Id}, Price={tg.Price}, Name={tg.Name}, MaxTicket={tg.MaxTicket}, SeatGroupId={tg.SeatGroupId}");
                    _context.ShowTimeTicketGroups.Add(new ShowTimeTicketGroup
                    {
                        ShowTimeId = showTime.Id,
                        Price = tg.Price,
                        Name = tg.Name,
                        MaxTicket = tg.MaxTicket,
                        SeatGroupId = tg.SeatGroupId,
                        TicketSaleStart = tg.TicketSaleStart,
                        TicketSaleEnd = tg.TicketSaleEnd
                    });
                    await _context.SaveChangesAsync();

                    Console.WriteLine("TickgetGroupId: " + tg.SeatGroupId);
                }

                // Lấy seats
                var listSeats = await _context.SeatingCharts
                    .Include(sc => sc.SeatGroups)
                    .ThenInclude(sg => sg.Seats)
                    .FirstOrDefaultAsync(sc => sc.EventId == st.EventId);

                if (listSeats != null)
                {
                    foreach (var seatGroup in listSeats.SeatGroups)
                    {
                        foreach (var seat in seatGroup.Seats)
                        {
                            _context.ShowTimeSeats.Add(new ShowTimeSeat
                            {
                                SeatId = seat.SeatId,
                                ShowTimeId = showTime.Id,
                                IsBooked = false
                            });
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Success" });
        }
        catch (DbUpdateException ex)
        {
            return BadRequest("Lỗi DB: " + ex.Message + " | " + ex.InnerException?.Message);
        }
        catch (Exception ex)
        {
            return BadRequest("Lỗi: " + ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SavePayment([FromForm] Event e)
    {
        var ev = await _context.Events.FirstOrDefaultAsync(s => s.EventId == e.EventId);
        if (ev == null) return NotFound();

        ev.BankNumber = e.BankNumber;
        ev.BankName = e.BankName;
        ev.EventStatus = EventStatus.Pending; // Hoàn tất

        await _context.SaveChangesAsync();
        return Ok(new { message = "Success" });
    }
    [Authorize]
    [Route("Event/Details/{id}/{method?}")]
    public async Task<IActionResult> Details(int id, string? method)
    {
        if (method != null && method.ToLower() == "update")
        {
            TempData["Method"] = "Update";
        }
        var ev = await _context.Events
                    .Include(e => e.ShowTimes)
                        .ThenInclude(e => e.ShowTimeTicketGroups)
                    // .Include(e => e.SeatingChart)
                    //     .ThenInclude(e => e.SeatGroups)
                    //         .ThenInclude(e => e.Seats)
                    .FirstOrDefaultAsync(e => e.EventId == id);
        if (ev == null)
        {
            Console.WriteLine("Khong tim thay");
            return NotFound();
        }
        Console.WriteLine("Detail:" + ev);
        return View(ev);
    }
    [Authorize]
    public async Task<IActionResult> BookingTicket(int id, int stId,string method)
    {
        if (method != null && method.ToLower() == "update")
        {
            TempData["Method"] = "Update";
        }
        var bookingModel = new ListBookingModelView();
        var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
        var seatingChart = await _context.SeatingCharts
                            .Include(s => s.SeatGroups)
                                .ThenInclude(s => s.Seats)
                            .FirstOrDefaultAsync(s => s.EventId == id);

        var showTimes = new ShowTime();
        if (stId == 0)
        {
            showTimes = await _context.ShowTimes
                    .Include(st => st.ShowTimeTicketGroups)
                    .Include(st => st.ShowTimeSeats)
                    .Where(st => st.EventId == id && st.ShowTimeTicketGroups.Any())
                    .FirstOrDefaultAsync();

        }
        else
        {


            showTimes = await _context.ShowTimes
                                           .Include(st => st.ShowTimeTicketGroups)
                                           .Include(st => st.ShowTimeSeats)
                                           .FirstOrDefaultAsync(st => st.EventId == id && st.Id == stId);
        }
        bookingModel._event = ev;
        bookingModel.SeatingChart = seatingChart;
        bookingModel.ShowTimes = showTimes;

        TempData["EventId"] = id;
        // if (stId == 0)
        // {

        // }
        return View("Booking", bookingModel);
    }

    [HttpPost]
    public async Task<IActionResult> SaveSeatStatus(string seatsJson, int id)
    {
        var seats = JsonConvert.DeserializeObject<List<ShowTimeSeatDto>>(seatsJson);
        var seatIds = seats?.Select(s => s.SeatId).ToList();
        var seatEntities = await _context.ShowTimeSeats
        .Where(s => seatIds!.Contains(s.SeatId))
        .ToListAsync();

        TempData["EventId"] = id;
        foreach (var seat in seatEntities)
        {
            if (seat.IsBooked)
            {
                return BadRequest("Ghế này hiện đang có người đặt xin vui lòng đặt ghế khác");
            }
            seat.IsBooked = true;
            // Console.WriteLine("da Dat");
        }

        await _context.SaveChangesAsync();

        TempData["SeatsJson"] = seatsJson;
        return RedirectToAction("Index", "Payment");

    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var deleteValid = await _event.Delete(id);
        if (deleteValid)
        {
            return Json(new { success = "Xóa thành công" });
        }
        else
        {
            return Json(new { err = "Xóa không thành công" });

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
