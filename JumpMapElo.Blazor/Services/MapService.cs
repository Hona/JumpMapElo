using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using JumpMapElo.Blazor.Models;

namespace JumpMapElo.Blazor.Services
{
    public class MapService
    {
        private readonly HttpClient _httpClient;
        private readonly Random _random;

        private List<Map> _maps;

        private Dictionary<int, Class> _classes; 

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<Map>> GetMapsAsync(Class jumpClass)
        {
            if (_maps != null && _classes != null)
            {
                if (jumpClass == Class.Both)
                {
                    return _maps;
                }

                var classMapIds = _classes.Where(x => x.Value == jumpClass || x.Value == Class.Both).Select(x => x.Key).ToList();
                
                return _maps.Where(x => classMapIds.Contains(x.Id)).ToList();
            }

            if (_maps == null)
            {
                var mapsResponseString = await _httpClient.GetStringAsync("https://tempus.xyz/api/maps/list");

                _maps = (JsonSerializer.Deserialize<List<Map>>(mapsResponseString, _jsonSerializerOptions) ?? throw new Exception("Error getting tempus maps"))
                    .ToList();
            }

            if (_classes == null)
            {
                var classesResponseString = await _httpClient.GetStringAsync("https://raw.githubusercontent.com/Hona/TempusHub/master/src/TempusHubBlazor/wwwroot/MapClasses.csv");

                _classes = new Dictionary<int, Class>();

                foreach (var classLine in classesResponseString.Trim().Split("\n"))
                {
                    var parts = classLine.Split(',');
                    var jumpClassParsed = parts[0].ToUpper() switch
                    {
                        "S" => Class.Soldier,
                        "D" => Class.Demoman,
                        _ => Class.Both
                    };

                    if (parts.Length != 2)
                    {
                        throw new Exception();
                    }

                    var foundMap = _maps.FirstOrDefault(x => x.Name.Equals(parts[1], StringComparison.OrdinalIgnoreCase));

                    if (foundMap == null)
                    {
                        continue;
                    }
                    
                    _classes.Add(foundMap.Id, jumpClassParsed);
                }
            }


            return await GetMapsAsync(jumpClass);
        }

        public Class GetMapClass(int mapId)
        {
            return _classes[mapId];
        }

        public async Task<Map> GetRandomMapAsync(Class jumpClass)
        {
            var maps = await GetMapsAsync(jumpClass);

            return maps[_random.Next(maps.Count)];
        }

        public async Task<(Map, Map)> GetTwoRandomMapsAsync(Class jumpClass)
        {
            Map firstMap;
            Map secondMap;

            do
            {
                firstMap = await GetRandomMapAsync(jumpClass);
                secondMap = await GetRandomMapAsync(jumpClass);
            } while (firstMap.Id == secondMap.Id);
            
            return (firstMap, secondMap);
        }

        public MapService(HttpClient httpClient, Random random)
        {
            _httpClient = httpClient;
            _random = random;
        }
    }
}