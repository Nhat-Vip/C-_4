public interface IAccountService
{
    Task<User?> Login(string email, string password);
    Task<bool> SignUp(User user);
}