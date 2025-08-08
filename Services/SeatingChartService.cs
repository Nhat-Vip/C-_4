
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
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Loi Khac:" + ex.Message);
            return false;
        }
    }

    public async Task<List<SeatingChart>> GetAll()
    {
        return await _context.SeatingCharts.ToListAsync();
    }
}