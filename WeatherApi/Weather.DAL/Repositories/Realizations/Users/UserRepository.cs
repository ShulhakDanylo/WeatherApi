using Microsoft.EntityFrameworkCore;
using Weather.DAL.Data;
using Weather.DAL.Entities;
using Weather.DAL.Repositories.Interfaces.Users;
using Weather.DAL.Repositories.Realizations.Base;

namespace Weather.DAL.Repositories.Realizations.Users;

public class UserRepository : RepositoryBase<User>, IUsersRepository
{
    public UserRepository(WeatherDbContext _dbContext) 
        : base(_dbContext)
    {
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}