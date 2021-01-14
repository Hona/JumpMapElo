using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JumpMapElo.Blazor.Models;
using JumpMapElo.Blazor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace JumpMapElo.Blazor.Api
{
    [Route("api")]
    [ApiController]
    public class ApiController : Controller
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly MapService _mapService;

        public ApiController(IRatingRepository ratingRepository, MapService mapService)
        {
            _ratingRepository = ratingRepository;
            _mapService = mapService;
        }

        [HttpGet("elos")]
        public async Task<IActionResult> GetEloCsvAsync([FromQuery] string jumpClassString = null, [FromQuery]string outputFormat = "json")
        {
            Class? jumpClass;
            if (jumpClassString == null)
            {
                jumpClass = null;
            }
            else if (!Enum.TryParse(jumpClassString, true, out Class jumpClassTemp))
            {
                return BadRequest($"Invalid jump class, valid options are - nothing for every map, or: '{string.Join(", ", Enum.GetNames(typeof(Class)))}'");
            }
            else
            {
                jumpClass = jumpClassTemp;
            }

            
            outputFormat = outputFormat.ToLower();
            if (outputFormat != "json" && outputFormat != "csv")
            {
                return BadRequest("Invalid output format, valid values are: 'json' or 'csv'");
            }

            var allMaps = await _ratingRepository.GetAll();
            var mapClasses = await _mapService.GetMapClassesAsync();

            var output = allMaps.ToList();

            if (jumpClass.HasValue)
            {
                output = output.Where(x => mapClasses[x.MapId] == jumpClass)
                    .ToList();
            }

            if (outputFormat == "json")
            {
                return Ok(output);
            }

            var outputString = "Map Name,Map Tempus ID,Elo" + Environment.NewLine +
                               string.Join(Environment.NewLine, output.Select(x => $"{x.MapName},{x.MapId},{x.Elo}"));
            
            return File(Encoding.UTF8.GetBytes(outputString), "text/csv", "jump_map_elo.csv");
        }
    }
}