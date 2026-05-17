using AutoMapper;
using Weather.DAL.Entities;
using Weather.BLL.DTOs.Users;

namespace Weather.BLL.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {

        CreateMap<RegisterUserRequestDto,User>();
        CreateMap<User, UserResponseDto>();

    }
}