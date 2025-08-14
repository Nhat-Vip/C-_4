
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
        try
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
                return true;
            }
            Console.WriteLine("Khong tim thay Id");
            return false;
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