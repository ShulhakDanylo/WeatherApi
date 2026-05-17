using System.Linq;
using Microsoft.EntityFrameworkCore;
using Weather.DAL.Data;
using Weather.DAL.Entities;
using Weather.DAL.Repositories.Interfaces.WeatherRecords;
using Weather.DAL.Repositories.Realizations.Base;

namespace Weather.DAL.Repositories.Realizations.WeatherRecords;

public class WeatherRecordRepository
    : RepositoryBase<WeatherRecord>,
        IWeatherRecordRepository
{
    public WeatherRecordRepository(WeatherDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<WeatherRecord>> GetByRegionAndPeriodAsync(
        int regionId,
        DateTime from,
        DateTime to)
    {
        return await _dbContext.WeatherRecords
            .Where(x =>
                x.RegionId == regionId &&
                x.RecordedAt >= from &&
                x.RecordedAt <= to)
            .AsNoTracking()
            .ToListAsync();
    }
}