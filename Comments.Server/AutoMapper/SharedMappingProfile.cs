using AutoMapper;
using Comments.Server.Data.Entities;
using Comments.Server.Models.Dtos;

namespace Comments.Server.AutoMapper;

public class SharedMappingProfile : Profile
{
    public SharedMappingProfile()
    {
        CreateMap<UserForRegistrationDto, User>();
    }
}
