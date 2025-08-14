
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly MyDbContext _context;
    public UserService(MyDbContext context) {
        _context = context;
    }
    public async Task<bool> AcceptRefund(int id)
    {
        try
        {
            var refund = await _context.Refunds.FirstOrDefaultAsync(r => r.TicketId == id);
            if (refund != null)
            {
                refund.Status = RefundStatus.Completed;
                _context.Refunds.Update(refund);
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
            Console.WriteLine("Loi Khac:" + ex.Message);
            return false;
        }
    }

    public async Task<bool> CancelRefund(int id)
    {
        try
        {
            var refund = await _context.Refunds.FirstOrDefaultAsync(r => r.TicketId == id);
            if (refund != null)
            {
                refund.Status = RefundStatus.Rejected;
                _context.Refunds.Update(refund);
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
            Console.WriteLine("Loi Khac:" + ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteUser(int id)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(r => r.UserId == id);
            if (user != null)
            {
                _context.Users.Remove(user);
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
            Console.WriteLine("Loi Khac:" + ex.Message);
            return false;
        }
    }

    public async Task<List<Ticket>?> GetTickets(int id)
    {
        return await _context.Tickets.Where(tk => tk.EventId == id).ToListAsync();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(us => us.UserId == id);
    }

    public async Task<bool> UpdateUser(User user)
    {
        try
        {
            var us = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (us == null) return false;
            us.PassWord = user.PassWord ?? us.PassWord;

            us.UserName = user.UserName;
            us.Email = user.Email;
            us.PhoneNumber = user.PhoneNumber;
            us.Avatar = user.Avatar ?? us.Avatar;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Loi DB: " + ex.Message);
            Console.WriteLine("Chi tiet: " + ex);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Loi Khac:" + ex.Message);
            return false;
        }
    }
}