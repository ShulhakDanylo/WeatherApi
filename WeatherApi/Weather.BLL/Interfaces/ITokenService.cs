using Weather.DAL.Entities;
namespace Weather.BLL.Interfaces;


public interface ITokenService
{
    string GenerateJwtToken(User user);
}