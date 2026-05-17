using Weather.DAL.Data;
using Weather.DAL.Entities;
using Weather.DAL.Repositories.Interfaces.Regions;
using Weather.DAL.Repositories.Realizations.Base;

namespace Weather.DAL.Repositories.Realizations.Regions;

public class RegionRepository
    : RepositoryBase<Region>,
        IRegionRepository
{
    public RegionRepository(WeatherDbContext context)
        : base(context)
    {
    }
}