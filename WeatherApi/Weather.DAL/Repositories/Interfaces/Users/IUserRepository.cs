using Weather.DAL.Entities;
using Weather.DAL.Repositories.Interfaces.Base;

namespace Weather.DAL.Repositories.Interfaces.Users;

public interface IUsersRepository : IRepositoryBase<User>
{
    Task<User?> GetByEmailAsync(string email);
}
