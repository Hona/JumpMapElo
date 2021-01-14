using System;
using System.Threading.Tasks;
using JumpMapElo.Blazor.Models;

namespace JumpMapElo.Blazor.Services
{
    public class MapRaterService
    {
        private readonly IRatingRepository _ratingRepository;

        public MapRaterService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        private async Task<MapRating> GetMapRatingEnsureCreated(Map map)
        {
            var existing = await _ratingRepository.GetById(map.Id);

            if (existing == null)
            {
                existing = new MapRating
                {
                    Elo = 1000,
                    MapId = map.Id,
                    MapName = map.Name
                };
                await _ratingRepository.Add(existing);
            }

            return existing;
        }

        public async Task VoteMapAsync(Map winningMap, Map losingMap)
        {
            var winning = await GetMapRatingEnsureCreated(winningMap);
            var losing = await GetMapRatingEnsureCreated(losingMap);

            var winningElo = winning.Elo;
            var losingElo = losing.Elo;
            EloHelper.CalculateElo(ref winningElo, ref losingElo, Outcome.Win);

            winning.Elo = winningElo;
            losing.Elo = losingElo;

            await _ratingRepository.Update(winning);
            await _ratingRepository.Update(losing);
        }
    }
}