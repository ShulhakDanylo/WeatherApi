using Weather.DAL.Entities;
using Weather.DAL.Repositories.Interfaces.Base;

namespace Weather.DAL.Repositories.Interfaces.WeatherRecords;

public interface IWeatherRecordRepository
    : IRepositoryBase<WeatherRecord>
{
    Task<IEnumerable<WeatherRecord>> GetByRegionAndPeriodAsync(
        int regionId,
        DateTime from,
        DateTime to);
}