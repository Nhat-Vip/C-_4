
using Microsoft.EntityFrameworkCore;

public class EventService : IEventService
{
    private readonly MyDbContext _context;
    public EventService(MyDbContext context) {
        _context = context;
    }

    public async Task<bool> Create(Event _event)
    {
        try
        {
            _context.Events.Add(_event);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Loi DB: " + ex.Message);
            Console.WriteLine("Chi tiết: " + ex.InnerException?.Message);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Loi Khac: " + ex.Message);
            return false;
        }
    }

    // public async Task<bool> CreateShowTime(List<ShowTime> showTimes)
    // {
    //     try
    //     {
    //         foreach (var st in showTimes)
    //         {
    //             var showTime = new ShowTime
    //             {
    //                 StartTime = st.StartTime,
    //                 EndTime = st.EndTime,
    //                 EventId = st.EventId
    //             };
    //             _context.ShowTimes.Add(showTime);
    //             await _context.SaveChangesAsync();
    //             foreach (var tk in st.ShowTimeTicketGroups)
    //             {
                    
    //             }
    //         }

    //     }
    //     catch (DbUpdateException ex)
    //     {
    //         Console.WriteLine("Loi DB: " + ex.Message);
    //         Console.WriteLine("Chi tiết: " + ex.InnerException?.Message);
    //         return false;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine("Loi Khac: " + ex.Message);
    //         return false;
    //     }
    // }

    public async Task<bool> Delete(int id)
    {
        var ev = await _context.Events
            .Include(e => e.ShowTimes)
                .ThenInclude(st => st.ShowTimeSeats)
                    .ThenInclude(sts => sts.TicketDetail)
            .Include(e => e.ShowTimes)
                .ThenInclude(st => st.ShowTimeTicketGroups)
            .Include(e => e.SeatingChart)
                .ThenInclude(sc => sc!.SeatGroups)
                    .ThenInclude(sg => sg.Seats)
            .FirstOrDefaultAsync(e => e.EventId == id);

        if (ev == null) return false;

        // Xóa TicketDetail
        foreach (var st in ev.ShowTimes)
        {
            foreach (var sts in st.ShowTimeSeats)
            {
                if (sts.TicketDetail != null)
                    _context.TicketDetails.Remove(sts.TicketDetail);
            }
        }

        // Xóa ShowTimeSeats & ShowTimeTicketGroups
        foreach (var st in ev.ShowTimes)
        {
            _context.ShowTimeSeats.RemoveRange(st.ShowTimeSeats);
            _context.ShowTimeTicketGroups.RemoveRange(st.ShowTimeTicketGroups);
        }

        // Xóa ShowTimes
        _context.ShowTimes.RemoveRange(ev.ShowTimes);

        // Xóa Seats → SeatGroups → SeatingChart
        if (ev.SeatingChart != null)
        {
            foreach (var sg in ev.SeatingChart.SeatGroups)
            {
                _context.Seats.RemoveRange(sg.Seats);
            }
            _context.SeatGroups.RemoveRange(ev.SeatingChart.SeatGroups);
            _context.SeatingCharts.Remove(ev.SeatingChart);
        }

        // Xóa Event
        _context.Events.Remove(ev);

        await _context.SaveChangesAsync();
        return true;
    }

public async Task<List<Event>> GetAll()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task<Event?> GetById(int Id)
    {
        return await _context.Events
        .FirstOrDefaultAsync(e => e.EventId == Id);
    }

    public async Task<List<Event>> LazyLoading(int take, int skip)
    {
        return await _context.Events.Skip(skip).Take(take).ToListAsync();
    }

    public async Task<bool> Update(Event _event)
    {
        try
        {
            _context.Events.Update(_event);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Loi DB: " + ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Loi Khac: " + ex.Message);
            return false;
        }
    }
}