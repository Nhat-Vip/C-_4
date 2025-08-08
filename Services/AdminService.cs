
using Microsoft.EntityFrameworkCore;

public class AdminService : IAdminService
{
    private readonly MyDbContext _context;
    public AdminService(MyDbContext context)
    {
        _context = context;
    }
    public async Task<bool> AcceptEvent(int id)
    {
        try
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
            if (ev != null)
            {
                ev.EventStatus = EventStatus.Approved;
                await _context.SaveChangesAsync();
                return true;
            }
            Console.WriteLine("ID khong chinh xac");
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

    public async Task<bool> CancelEvent(int id)
    {
        try
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
            if (ev != null)
            {
                ev.EventStatus = EventStatus.Cancel;
                await _context.SaveChangesAsync();
                return true;
            }
            Console.WriteLine("ID khong chinh xac");
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

    public async Task<List<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }
}