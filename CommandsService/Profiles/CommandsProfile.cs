using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>().ReverseMap();
            CreateMap<CommandReadDto, Command>().ReverseMap();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(dest => dest.ExternalId, 
                    src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

        }
    }
}