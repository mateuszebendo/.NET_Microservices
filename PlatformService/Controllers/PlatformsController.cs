using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo platformRepo, 
                                   IMapper mapper, 
                                   ICommandDataClient commandDataClient,
                                   IMessageBusClient messageBusClient)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        [Route("get-all-platforms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllPlatforms()
        {
            Console.WriteLine("--> Getting Platforms...");
            
            var enumerableList = _platformRepo.GetAllPlatforms();
            
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(enumerableList));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = _platformRepo.GetPlatformById(id);
            if (platformItem == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PlatformReadDto>(platformItem));
        }


        [HttpPost]
        [Route("add-platform")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform([FromBody] PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine("--> Creating Platform...");
            
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            
            _platformRepo.CreatePlatform(platformModel);
            _platformRepo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);
            
            //Send Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not send synchronously: " + ex.Message);
            }
            
            //Send Async Message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Could not send asynchronously: " + ex.Message);
            }
            
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id} , platformReadDto);
        }
        
    }
}