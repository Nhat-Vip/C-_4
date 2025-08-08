public interface IAdminService
{
    Task<List<User>> GetAll();
    Task<bool> AcceptEvent(int id);
    Task<bool> CancelEvent(int id);
    
}