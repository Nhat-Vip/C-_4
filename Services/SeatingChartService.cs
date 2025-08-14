
using Microsoft.EntityFrameworkCore;

public class SeatingchartService : ISeatingChartService
{
    private readonly MyDbContext _context;
    public SeatingchartService(MyDbContext context) {
        _context = context;
    }
    public async Task<bool> Create(SeatingChart seatingChart)
    {
        try
        {
            _context.SeatingCharts.Add(seatingChart);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Loi DB: " + ex.Message);
            Console.WriteLine("Chi tiet loi: " + ex.InnerException?.Message);
            Console.WriteLine("Full loi: " + ex.ToString());
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khác:");
            Console.WriteLine("Message: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);
            return false;
        }
    }

    public async Task<List<SeatingChart>> GetAll()
    {
        return await _context.SeatingCharts.ToListAsync();
    }
}