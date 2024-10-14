using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
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

        [HttpGet("{commandId}")]
        public ActionResult<CommandReadDto> GetCommandByPlatformId([FromRoute] int platformId, [FromRoute] int commandId)
        {
            Console.WriteLine($"--> Getting command by platform ID = {platformId}...");

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<CommandReadDto>(_repository.GetCommand(platformId, commandId)));
        }
        
        /*[HttpPost("{platformId}/commands")]
        public ActionResult CreateCommand([FromRoute] int platformId, [FromBody] CommandCreateDto command)
        {
            Console.WriteLine($"--> Creating command with platform ID = {platformId}...");

            return Ok(command);
        }*/
    }
}