using AutoMapper;
using Comments.Server.Data.Entities;
using Comments.Server.Models.Dtos;

namespace Comments.Server.AutoMapper;

public class SharedMappingProfile : Profile
{
    public SharedMappingProfile()
    {
        CreateMap<User, UserGetDto>();

        CreateMap<Comment, CommentGetDto>()
            .ForMember(
                dest => dest.Replies, 
                opt => opt.MapFrom(src => src.Replies));
    }
}
