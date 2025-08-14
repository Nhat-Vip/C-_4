public interface IEventService
{
    Task<List<Event>> GetAll();
    Task<bool> Create(Event _event);
    // Task<bool> CreateShowTime(List<ShowTime> showTime);
    Task<bool> Delete(int id);
    Task<bool> Update(Event _event);
    Task<Event?> GetById(int Id);
    Task<List<Event>> LazyLoading(int take, int skip);
}