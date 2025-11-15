using Microsoft.EntityFrameworkCore;
using System;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = serviceProvider.GetRequiredService<MyDbContext>();

        try
        {
            
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Nếu đã có dữ liệu User hoặc Event thì bỏ qua
            if (!context.Users.Any())
            {
            
                // 1. Thêm Admin User
                var admin = new User
                {
                    UserName = "Admin",
                    Email = "admin@gmail.com",
                    Role = Role.Admin,
                    PhoneNumber = "09998885432",
                    PassWord = "Admin@123"
                };
                context.Users.Add(admin);
                context.SaveChanges();
            }

            if(!context.Events.Any())
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine("Bắt đầu khởi tạo dữ liệu Events...");
                Console.ResetColor();
                var admin = context.Users.First(u => u.Role == Role.Admin);
                // Tạo 10 sự kiện
                var events = new List<Event>
                {
                    // 1. Concert - Sơn Tùng M-TP
                    new Event
                    {
                        EventName = "Sky Tour 2025 - Sơn Tùng M-TP",
                        EventAddress = "Sân vận động Mỹ Đình, Hà Nội",
                        Image = "/Images/anh1.jpeg",
                        SubImage = "/images/sontung-sub.jpg",
                        Description = "Concert hoành tráng nhất năm của Sơn Tùng M-TP với dàn sao khách mời.",
                        StartEvent = new DateTime(2025, 12, 20, 19, 0, 0),
                        EndDateTime = new DateTime(2025, 12, 20, 23, 0, 0),
                        EventType = EventType.Concert,
                        EventStatus = EventStatus.Upcoming,
                        BankNumber = "1234567890",
                        BankName = "Vietcombank",
                        UserId = admin.UserId,
                        SeatingChart = new SeatingChart
                        {
                            PosX = 0,
                            PosY = 0,
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup
                                {
                                    Name = "VIP A",
                                    Color = "#FFD700",
                                    Rotate = "0",
                                    PosX = 100,
                                    PosY = 50,
                                    Cols = 10,
                                    Rows = 5,
                                    Seats = GenerateSeats(10, 5, "A")
                                },
                                new SeatGroup
                                {
                                    Name = "Thường B",
                                    Color = "#4CAF50",
                                    Rotate = "0",
                                    PosX = 100,
                                    PosY = 300,
                                    Cols = 20,
                                    Rows = 15,
                                    Seats = GenerateSeats(20, 15, "B")
                                }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2025, 12, 20, 19, 0, 0),
                                EndTime = new DateTime(2025, 12, 20, 23, 0, 0),
                                // ShowTimeTicketGroups = new List<ShowTimeTicketGroup>
                                // {
                                //     new ShowTimeTicketGroup { Price = 2500000, Name = "VIP A", MaxTicket = 50, TicketSaleStart = DateTime.Now.AddDays(-30), TicketSaleEnd = new DateTime(2025, 12, 19) },
                                //     new ShowTimeTicketGroup { Price = 800000, Name = "Thường B", MaxTicket = 300, TicketSaleStart = DateTime.Now.AddDays(-30), TicketSaleEnd = new DateTime(2025, 12, 19) }
                                // }
                            }
                        }
                    },

                    // 2. Sport - V-League Final
                    new Event
                    {
                        EventName = "Chung kết V-League 2025: Hà Nội FC vs Viettel FC",
                        EventAddress = "Sân vận động Hàng Đẫy, Hà Nội",
                        Image = "/Images/anh2.jpg",
                        Description = "Trận chung kết kịch tính nhất mùa giải V-League 2025.",
                        StartEvent = new DateTime(2025, 11, 30, 18, 0, 0),
                        EndDateTime = new DateTime(2025, 11, 30, 20, 30, 0),
                        EventType = EventType.Sport,
                        EventStatus = EventStatus.Upcoming,
                        BankNumber = "0987654321",
                        BankName = "BIDV",
                        UserId = admin.UserId,
                        SeatingChart = new SeatingChart
                        {
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup { Name = "Khán đài A", Color = "#FF5722", PosX = 50, PosY = 50, Cols = 15, Rows = 20, Seats = GenerateSeats(15, 20, "A") },
                                new SeatGroup { Name = "Khán đài B", Color = "#2196F3", PosX = 400, PosY = 50, Cols = 15, Rows = 20, Seats = GenerateSeats(15, 20, "B") }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2025, 11, 30, 18, 0, 0),
                                EndTime = new DateTime(2025, 11, 30, 20, 30, 0),
                                // ShowTimeTicketGroups = new List<ShowTimeTicketGroup>
                                // {
                                //     new ShowTimeTicketGroup { Price = 500000, Name = "Khán đài A", MaxTicket = 300, TicketSaleStart = DateTime.Now.AddDays(-15), TicketSaleEnd = new DateTime(2025, 11, 29)},
                                //     new ShowTimeTicketGroup { Price = 300000, Name = "Khán đài B", MaxTicket = 300, TicketSaleStart = DateTime.Now.AddDays(-15), TicketSaleEnd = new DateTime(2025, 11, 29) }
                                // }
                            }
                        }
                    },

                    // 3. Concert - BlackPink (Quá khứ)
                    new Event
                    {
                        EventName = "Born Pink World Tour - Hanoi Stop",
                        EventAddress = "Phố đi bộ Nguyễn Huệ, TP.HCM",
                        Image = "/Images/anh3.jpg",
                        Description = "BlackPink trở lại Việt Nam sau 3 năm.",
                        StartEvent = new DateTime(2025, 1, 15, 19, 30, 0),
                        EndDateTime = new DateTime(2025, 1, 15, 23, 0, 0),
                        EventType = EventType.Concert,
                        EventStatus = EventStatus.Upcoming,
                        BankNumber = "1122334455",
                        BankName = "Techcombank",
                        UserId = admin.UserId,
                        SeatingChart = new SeatingChart
                        {
                            PosX = 0, PosY = 0,
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup { Name = "VIP", Color = "#FFD700", PosX = 100, PosY = 50, Cols = 8, Rows = 5, Seats = GenerateSeats(8, 5, "V") },
                                new SeatGroup { Name = "Thường", Color = "#4CAF50", PosX = 100, PosY = 250, Cols = 20, Rows = 10, Seats = GenerateSeats(20, 10, "T") }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2025, 1, 15, 19, 30, 0),
                                EndTime = new DateTime(2025, 1, 15, 23, 0, 0)
                            }
                        }
                    },

                    // 4. Sport - SEA Games 33 (Draft)
                    new Event
                    {
                        EventName = "SEA Games 33 - Bóng đá nam",
                        EventAddress = "Sân vận động Quốc gia Mỹ Đình",
                        Image = "/Images/anh4.jpg",
                        EventType = EventType.Sport,
                        EventStatus = EventStatus.Upcoming,
                        UserId = admin.UserId,

                        SeatingChart = new SeatingChart
                        {
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup { Name = "Khán đài A", Color = "#FF5722", PosX = 50, PosY = 50, Cols = 20, Rows = 25, Seats = GenerateSeats(20, 25, "A") },
                                new SeatGroup { Name = "Khán đài B", Color = "#2196F3", PosX = 450, PosY = 50, Cols = 20, Rows = 25, Seats = GenerateSeats(20, 25, "B") }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2025, 5, 20, 19, 0, 0),
                                EndTime = new DateTime(2025, 5, 20, 21, 30, 0)
                            }
                        }
                    },

                    // 5. Concert - Mỹ Tâm
                    new Event
                    {
                        EventName = "Tri Âm - Live Concert Mỹ Tâm 2025",
                        EventAddress = "Trung tâm Hội nghị Quốc gia, Hà Nội",
                        Image = "/Images/anh5.jpg",
                        Description = "Hành trình tri âm cùng diva Mỹ Tâm.",
                        StartEvent = new DateTime(2025, 12, 25, 20, 0, 0),
                        EndDateTime = new DateTime(2025, 12, 25, 23, 0, 0),
                        EventType = EventType.Concert,
                        EventStatus = EventStatus.Upcoming,
                        UserId = admin.UserId,

                        SeatingChart = new SeatingChart
                        {
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup { Name = "VIP", Color = "#FFD700", PosX = 150, PosY = 100, Cols = 10, Rows = 6, Seats = GenerateSeats(10, 6, "V") },
                                new SeatGroup { Name = "Thường", Color = "#4CAF50", PosX = 50, PosY = 300, Cols = 25, Rows = 12, Seats = GenerateSeats(25, 12, "T") }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2025, 12, 25, 20, 0, 0),
                                EndTime = new DateTime(2025, 12, 25, 23, 0, 0)
                            }
                        }
                    },

                    // 6. Sport - AFF Cup 2025
                    new Event
                    {
                        EventName = "AFF Cup 2025 - Việt Nam vs Thái Lan",
                        EventAddress = "Sân vận động Quốc gia, Hà Nội",
                        Image = "/Images/anh6.jpg",
                        StartEvent = new DateTime(2025, 12, 15, 19, 30, 0),
                        EndDateTime = new DateTime(2025, 12, 15, 22, 0, 0),
                        EventType = EventType.Sport,
                        EventStatus = EventStatus.Upcoming,
                        UserId = admin.UserId,

                        SeatingChart = new SeatingChart
                        {
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup { Name = "Khán đài A", Color = "#FF5722", PosX = 50, PosY = 50, Cols = 18, Rows = 22, Seats = GenerateSeats(18, 22, "A") },
                                new SeatGroup { Name = "Khán đài B", Color = "#2196F3", PosX = 420, PosY = 50, Cols = 18, Rows = 22, Seats = GenerateSeats(18, 22, "B") }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2025, 12, 15, 19, 30, 0),
                                EndTime = new DateTime(2025, 12, 15, 22, 0, 0)
                            }
                        }
                    },

                    // 7. Concert - Hài Tết 2025
                    new Event
                    {
                        EventName = "Gala Cười 2025 - Xuân Bắc, Tự Long, Vân Dung",
                        EventAddress = "Nhà hát Lớn Hà Nội",
                        Image = "/Images/anh2.jpg",
                        StartEvent = new DateTime(2026, 1, 28, 20, 0, 0),
                        EndDateTime = new DateTime(2026, 1, 28, 22, 30, 0),
                        EventType = EventType.Concert,
                        EventStatus = EventStatus.Upcoming,
                        UserId = admin.UserId,

                        SeatingChart = new SeatingChart
                        {
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup { Name = "Hạng A", Color = "#FFD700", PosX = 100, PosY = 100, Cols = 12, Rows = 8, Seats = GenerateSeats(12, 8, "A") },
                                new SeatGroup { Name = "Hạng B", Color = "#4CAF50", PosX = 100, PosY = 300, Cols = 15, Rows = 10, Seats = GenerateSeats(15, 10, "B") }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2026, 1, 28, 20, 0, 0),
                                EndTime = new DateTime(2026, 1, 28, 22, 30, 0)
                            }
                        }
                    },

                    // 8. Sport - Marathon Hanoi 2025
                    new Event
                    {
                        EventName = "VnExpress Marathon Hanoi 2025",
                        EventAddress = "Hồ Hoàn Kiếm, Hà Nội",
                        Image = "/Images/anh5.jpg",
                        StartEvent = new DateTime(2025, 11, 23, 5, 0, 0),
                        EndDateTime = new DateTime(2025, 11, 23, 12, 0, 0),
                        EventType = EventType.Sport,
                        EventStatus = EventStatus.Upcoming,
                        UserId = admin.UserId,

                        SeatingChart = new SeatingChart
                        {
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup { Name = "Khu vực VIP", Color = "#FFD700", PosX = 100, PosY = 50, Cols = 10, Rows = 5, Seats = GenerateSeats(10, 5, "V") },
                                new SeatGroup { Name = "Khu vực Thường", Color = "#4CAF50", PosX = 100, PosY = 200, Cols = 30, Rows = 15, Seats = GenerateSeats(30, 15, "T") }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2025, 11, 23, 5, 0, 0),
                                EndTime = new DateTime(2025, 11, 23, 12, 0, 0)
                            }
                        }
                    },

                    // 9. Concert - Cancelled
                    new Event
                    {
                        EventName = "K-Pop Festival 2025 (Đã hủy)",
                        EventAddress = "Sân vận động Quân khu 7, TP.HCM",
                        Image = "/Images/anh3.jpg",
                        EventType = EventType.Concert,
                        EventStatus = EventStatus.Upcoming,
                        UserId = admin.UserId,
                        SeatingChart = new SeatingChart
                        {
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup { Name = "VIP", Color = "#FFD700", PosX = 150, PosY = 100, Cols = 10, Rows = 6, Seats = GenerateSeats(10, 6, "V") },
                                new SeatGroup { Name = "Thường", Color = "#4CAF50", PosX = 50, PosY = 300, Cols = 25, Rows = 12, Seats = GenerateSeats(25, 12, "T") }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2025, 10, 10, 18, 0, 0),
                                EndTime = new DateTime(2025, 10, 10, 22, 0, 0)
                            }
                        }
                    },

                    // 10. Sport - Cúp Quốc Gia
                    new Event
                    {
                        EventName = "Tứ kết Cúp Quốc Gia: HAGL vs SLNA",
                        EventAddress = "Sân Pleiku, Gia Lai",
                        Image = "/Images/anh6.jpg",
                        StartEvent = new DateTime(2025, 11, 18, 17, 0, 0),
                        EndDateTime = new DateTime(2025, 11, 18, 19, 30, 0),
                        EventType = EventType.Sport,
                        EventStatus = EventStatus.Upcoming,
                        UserId = admin.UserId,
                        SeatingChart = new SeatingChart
                        {
                            SeatGroups = new List<SeatGroup>
                            {
                                new SeatGroup { Name = "Khán đài A", Color = "#FF5722", PosX = 50, PosY = 50, Cols = 16, Rows = 20, Seats = GenerateSeats(16, 20, "A") },
                                new SeatGroup { Name = "Khán đài B", Color = "#2196F3", PosX = 400, PosY = 50, Cols = 16, Rows = 20, Seats = GenerateSeats(16, 20, "B") }
                            }
                        },
                        ShowTimes = new List<ShowTime>
                        {
                            new ShowTime
                            {
                                StartTime = new DateTime(2025, 11, 18, 17, 0, 0),
                                EndTime = new DateTime(2025, 11, 18, 19, 30, 0)
                            }
                        }
                    }
                };

                // 3. Lưu Events + toàn bộ graph (SeatingChart, SeatGroup, Seat, ShowTime, ShowTimeTicketGroup)
                context.Events.AddRange(events);
                context.SaveChanges();

                // tạo ShowTimeTicketGroup
                var eventsWithChart = context.Events
                    .Include(e => e.SeatingChart!).ThenInclude(sc => sc.SeatGroups!)
                    .Include(e => e.ShowTimes!)
                    .Where(e => e.SeatingChart != null)
                    .ToList();

                foreach (var ev in eventsWithChart)
                {
                    // var st = ev.ShowTimes.First();
                    foreach(var st in ev.ShowTimes)
                    {
                        st.ShowTimeTicketGroups = new List<ShowTimeTicketGroup>();
                        foreach (var item in ev.SeatingChart!.SeatGroups)
                        {
                            st.ShowTimeTicketGroups.Add(
                                new ShowTimeTicketGroup
                                {
                                    Price = 2500000,
                                    Name = item.Name,
                                    MaxTicket = 50,
                                    SeatGroup = item,
                                    ShowTime = st
                                }
                            );
                        }
                    }
                }

                // 4. Gán quan hệ navigation (EF tự sinh FK)
                var savedEvents = context.Events
                    .Include(e => e.SeatingChart)
                        .ThenInclude(sc => sc!.SeatGroups)
                            .ThenInclude(sg => sg.Seats)
                    .Include(e => e.ShowTimes)
                    .Where(e => e.SeatingChart != null)
                    .ToList();

                foreach (var ev in savedEvents)
                {
                    // Gán Event ↔ SeatingChart
                    if (ev.SeatingChart != null)
                    {
                        ev.SeatingChart.Event = ev;
                        foreach (var sg in ev.SeatingChart.SeatGroups)
                        {
                            sg.SeatingChart = ev.SeatingChart;
                            foreach (var seat in sg.Seats)
                            {
                                seat.SeatGroup = sg;
                            }
                        }
                    }

                    foreach (var st in ev.ShowTimes)
                    {
                        st.Event = ev;

                        foreach (var sttg in st.ShowTimeTicketGroups)
                        {
                            var seatGroup = ev.SeatingChart!.SeatGroups.FirstOrDefault(g => g.Name == sttg.Name);
                            if (seatGroup != null)
                            {
                                sttg.ShowTime = st;
                                sttg.SeatGroup = seatGroup;
                                sttg.SeatGroupId = seatGroup.SeatGroupId;
                            }
                        }

                        // Tạo ShowTimeSeat
                        var showTimeSeats = ev.SeatingChart!.SeatGroups
                            .SelectMany(g => g.Seats)
                            .Select(seat => new ShowTimeSeat
                            {
                                ShowTime = st,
                                Seat = seat,
                                IsBooked = false
                            }).ToList();

                        st.ShowTimeSeats = showTimeSeats;
                    }
                }

                context.SaveChanges(); // Thành công!
            }
        }
        catch( Exception ex)
        {
            Console.WriteLine($"Lỗi khi khởi tạo dữ liệu: {ex.Message}");
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Lỗi khi seed dữ liệu");
            throw;   // ném lại để console hiện
        }
    }

    // Hàm hỗ trợ tạo ghế
    private static List<Seat> GenerateSeats(int cols, int rows, string prefix)
    {
        var seats = new List<Seat>();
        for (int row = 1; row <= rows; row++)
        {
            for (int col = 1; col <= cols; col++)
            {
                seats.Add(new Seat
                {
                    SeatName = $"{prefix}{row}-{col}",
                    PosX = col * 25,
                    PosY = row * 25
                });
            }
        }
        return seats;
    }
}