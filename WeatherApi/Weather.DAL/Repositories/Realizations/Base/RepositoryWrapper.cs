using Weather.DAL.Data;
using Weather.DAL.Repositories.Interfaces.Base;
using Weather.DAL.Repositories.Interfaces.Regions;
using Weather.DAL.Repositories.Interfaces.Users;
using Weather.DAL.Repositories.Interfaces.WeatherRecords;
using Weather.DAL.Repositories.Realizations.Regions;
using Weather.DAL.Repositories.Realizations.Users;
using Weather.DAL.Repositories.Realizations.WeatherRecords;

namespace Weather.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly WeatherDbContext _context;

    private IWeatherRecordRepository? _weatherRecords;
    private IRegionRepository? _regions;
    private IUsersRepository? _users;

    public RepositoryWrapper(WeatherDbContext context)
    {
        _context = context;
    }

    public IWeatherRecordRepository WeatherRecords =>
        _weatherRecords ??= new WeatherRecordRepository(_context);

    
    public IUsersRepository Users => 
        _users ??= new UserRepository(_context); 
    
    public IRegionRepository Regions => 
        _regions ??= new RegionRepository(_context);

    
    public void ClearChangeTracker()
    {
        _context.ChangeTracker.Clear();
    }

    public int SaveChanges() => _context.SaveChanges();

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}