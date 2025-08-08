public interface IUserService
{
    Task<User?> GetUserById(int id);
    Task<bool> DeleteUser(int id);
    Task<bool> UpdateUser(User user);
    Task<bool> AcceptRefund(int id);
    Task<bool> CancelRefund(int id);
    Task<List<Ticket>?> GetTickets(int id);
}