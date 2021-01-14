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

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<Map>> GetMapsAsync()
        {
            if (_maps != null)
            {
                return _maps;
            }

            var mapsResponseString = await _httpClient.GetStringAsync("https://tempus.xyz/api/maps/list");

            _maps = (JsonSerializer.Deserialize<List<Map>>(mapsResponseString, _jsonSerializerOptions) ?? throw new Exception("Error getting tempus maps"))
                .ToList();

            return await GetMapsAsync();
        }

        public async Task<Map> GetRandomMapAsync()
        {
            var maps = await GetMapsAsync();

            return maps[_random.Next(maps.Count)];
        }

        public async Task<(Map, Map)> GetTwoRandomMapsAsync()
        {
            Map firstMap;
            Map secondMap;

            do
            {
                firstMap = await GetRandomMapAsync();
                secondMap = await GetRandomMapAsync();
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