
using Microsoft.EntityFrameworkCore;

public class AccountService : IAccountService
{
    private readonly MyDbContext _context;
    public AccountService(MyDbContext context)
    {
        _context = context;
    }
    public async Task<User?> Login(string email, string password)
    {
        return await _context.Users.FirstOrDefaultAsync(us => us.Email == email && us.PassWord == password);
    }

    public async Task<bool> SignUp(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Lỗi DB: " + ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khác: " + ex.Message);
            return false;
        }
    }
}