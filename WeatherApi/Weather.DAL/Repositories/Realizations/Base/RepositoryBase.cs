using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Weather.DAL.Data;
using Weather.DAL.Repositories.Interfaces.Base;
using Weather.DAL.Repositories.Options;

namespace Weather.DAL.Repositories.Realizations.Base;

public class RepositoryBase<T> : IRepositoryBase<T>
    where T : class
{
    protected readonly WeatherDbContext _dbContext;

    protected RepositoryBase(WeatherDbContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? queryOptions = null)
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (queryOptions != null)
        {
            query = queryOptions.Apply(query);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(QueryOptions<T>? queryOptions = null)
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (queryOptions != null)
        {
            query = queryOptions.Apply(query);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        var result = await _dbContext.Set<T>().AddAsync(entity);
        return result.Entity;
    }

    public EntityEntry<T> Update(T entity)
    {
        return _dbContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }
}