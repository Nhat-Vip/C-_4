public interface ISeatingChartService
{
    Task<List<SeatingChart>> GetAll();
    Task<bool> Create(SeatingChart seatingChart);
    
}