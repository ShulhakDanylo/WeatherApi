using Weather.DAL.Repositories.Interfaces.Regions;
using Weather.DAL.Repositories.Interfaces.WeatherRecords;
using Weather.DAL.Repositories.Interfaces.Users;
namespace Weather.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    IWeatherRecordRepository WeatherRecords { get; }
    IRegionRepository Regions { get; }
    
    IUsersRepository Users { get; }

    void ClearChangeTracker();
    int SaveChanges();
    Task<int> SaveChangesAsync();
}