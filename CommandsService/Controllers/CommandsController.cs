using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [ApiController]
    [Route("api/commands/platforms/{platformId}/[controller]")]
    public class CommandsController : ControllerBase
    {
        
        //TODO: Adicionar migrations, reuppar a imagem no dockerhub, testar com microsoft server
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommandsByPlatformId([FromRoute] int platformId)
        {
            Console.WriteLine($"--> Getting command by platform ID = {platformId}...");
            
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(_repository.GetCommandsForPlatform(platformId)));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandByPlatformId([FromRoute] int platformId, [FromRoute] int commandId)
        {
            Console.WriteLine($"--> Getting command by platform ID = {platformId}...");

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<CommandReadDto>(_repository.GetCommand(platformId, commandId)));
        }
        
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform([FromRoute] int platformId, [FromBody] CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"--> Creating command with platform ID = {platformId}...");

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandCreateDto);
            command.PlatformId = platformId; 

            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute("GetCommandForPlatform", 
                new { platformId = platformId, commandId = commandReadDto.Id },
                commandReadDto);
        }
    }
}