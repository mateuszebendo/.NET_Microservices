using AutoMapper;
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
            CreateMap<Platform, GrpcPlatformModel>()
                .ForMember(dest => dest.PlatformId,
                    src
                        => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                    src
                        => src.MapFrom(src => src.Name))
                .ForMember(dest => dest.Publisher,
                    src
                        => src.MapFrom(src => src.Publisher));
        }
    }
}