﻿using AutoMapper;
using Entities.Models;
using Shared.Dtos;

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

        CreateMap<CommentCreateDto, Comment>();
        CreateMap<UserCreateDto, User>();
    }
}
