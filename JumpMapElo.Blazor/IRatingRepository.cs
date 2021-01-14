using System.Collections.Generic;
using System.Threading.Tasks;
using JumpMapElo.Blazor.Models;
using JumpMapElo.Blazor.Services;

namespace JumpMapElo.Blazor
{
    public interface IRatingRepository
    {
        Task Add(MapRating model);
        Task Update(MapRating model);
        Task Delete(MapRating model);

        Task<IReadOnlyList<MapRating>> GetAll();
        Task<MapRating> GetById(int mapId);
        Task<IReadOnlyList<MapRating>> GetByName(string name);
    }
}