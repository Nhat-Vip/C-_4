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
    public IActionResult Create()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.UserId = userId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SaveEventInfo([FromForm] Event ev,[FromForm] IFormFile ImageFile)
    {
        try
        {
            var e = await _event.GetById(ev.EventId);
            if (e != null)
            {
                e.EventName = ev.EventName;
                e.Description = ev.Description;
                e.StartEvent = ev.StartEvent;
                e.EndDateTime = ev.EndDateTime;
                e.EventType = ev.EventType;
                e.EventAddress = ev.EventAddress;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    e.Image = await SaveImg(ImageFile);
                }

                await _context.SaveChangesAsync();
                return Ok(new { eventId = e.EventId });
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
            // Cập nhật các thuộc tính của SeatingChart
            seatingChart.PosX = stc.PosX;
            seatingChart.PosY = stc.PosY;

            // Xóa các SeatGroup cũ (bao gồm Seats liên quan)
            foreach (var oldSeatGroup in seatingChart.SeatGroups)
            {
                _context.Seats.RemoveRange(oldSeatGroup.Seats); // Xóa Seats của SeatGroup
            }
            _context.SeatGroups.RemoveRange(seatingChart.SeatGroups); // Xóa SeatGroups

            // Thêm các SeatGroup mới từ stc
            seatingChart.SeatGroups = stc.SeatGroups ?? new List<SeatGroup>();
            foreach (var seatGroup in seatingChart.SeatGroups)
            {
                seatGroup.SeatingChartId = seatingChart.SeatingChartId; // Gán khóa ngoại
                foreach (var seat in seatGroup.Seats)
                {
                    seat.SeatGroupId = seatGroup.SeatGroupId; // Gán khóa ngoại
                }
            }

            // Cập nhật SeatingChart
            _context.SeatingCharts.Update(seatingChart);
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
            if (showTimes == null)
                return BadRequest("Dữ liệu gửi lên không hợp lệ");

            foreach (var st in showTimes)
            {
                Console.WriteLine($"ShowTime FE: Id={st.Id}, EventId={st.EventId}, Start={st.StartTime}, End={st.EndTime}, TicketGroupCount={st.ShowTimeTicketGroups?.Count}");
                ShowTime showTime;

                if (st.Id > 0) // UPDATE
                {
                    Console.WriteLine("Cap Nhat Show Time");
                    showTime = (await _context.ShowTimes
                        .Include(x => x.ShowTimeSeats)
                        .Include(x => x.ShowTimeTicketGroups)
                        .FirstOrDefaultAsync(x => x.Id == st.Id))!;

                    if (showTime == null) continue;

                    // Cập nhật thời gian
                    showTime.StartTime = st.StartTime;
                    showTime.EndTime = st.EndTime;

                    // Xóa toàn bộ ticket groups & seats cũ
                    _context.ShowTimeTicketGroups.RemoveRange(showTime.ShowTimeTicketGroups);
                    _context.ShowTimeSeats.RemoveRange(showTime.ShowTimeSeats);

                    // Lấy seats từ seating chart
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

                    // Thêm ticket groups mới
                    foreach (var tg in st.ShowTimeTicketGroups)
                    {
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
                    }
                }
                else // INSERT mới
                {
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
            }

            
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
    public async Task<IActionResult> Details(int id)
    {
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
    public async Task<IActionResult> BookingTicket(int id,int stId)
    {
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
                    .Include(st=>st.ShowTimeSeats)
                    .Where(st => st.EventId == id && st.ShowTimeTicketGroups.Any())
                    .FirstOrDefaultAsync();

        }
        else {
            

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

        // Console.WriteLine("seatIds: " + string.Join(",", seatIds));

        // Console.WriteLine("seatEntiti"+seatEntities);
        // if (seatEntities.Count <= 0)
        // {
        //     Console.WriteLine("Not found");
        //     return NotFound();
        // }
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
