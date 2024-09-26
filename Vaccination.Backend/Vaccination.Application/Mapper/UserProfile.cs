using AutoMapper;
using Vaccination.Application.Dtos.User;
using Vaccination.Domain.Entities;

namespace Vaccination.Application.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDetailsResponse>();
            CreateMap<UpdateUserRequest, User>();
            CreateMap<User, UpdateUserResponse>();
        }
    }
}