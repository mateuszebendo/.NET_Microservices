﻿using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformsProfile : Profile
    { 
        public PlatformsProfile()
        {
            CreateMap<Platform, PlatformReadDto>().ReverseMap();
            CreateMap<Platform, PlatformCreateDto>().ReverseMap();
            CreateMap<PlatformReadDto, PlatformPublishedDto>();
        }
    }
}