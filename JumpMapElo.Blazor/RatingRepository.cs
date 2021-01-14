using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JumpMapElo.Blazor.Models;
using Marten;

namespace JumpMapElo.Blazor
{
    public class RatingRepository : IRatingRepository
    {
        private readonly IDocumentStore _store;
        private SemaphoreSlim _semaphore = new(1, 1);

        public RatingRepository(IDocumentStore store)
        {
            _store = store;
        }

        public async Task Add(MapRating model)
        {
            await _semaphore.WaitAsync();
            try
            {
                using var session = _store.LightweightSession();
            
                session.Insert(model);

                await session.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _semaphore.Release();
                Console.WriteLine(e);
                throw;
            }

            _semaphore.Release();
        }

        public async Task Update(MapRating model)
        {
            await _semaphore.WaitAsync();
            try
            {
                using var session = _store.LightweightSession();
            
                session.Update(model);

                await session.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _semaphore.Release();
                Console.WriteLine(e);
                throw;
            }

            _semaphore.Release();
        }

        public async Task Delete(MapRating model)
        {
            using var session = _store.LightweightSession();
            
            session.Delete(model);

            await session.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<MapRating>> GetAll()
        {
            using var session = _store.QuerySession();

            return await session.Query<MapRating>()
                .ToListAsync();
        }

        public async Task<MapRating> GetById(int mapId)
        {
            using var session = _store.QuerySession();
            
            return await session.Query<MapRating>()
                .SingleOrDefaultAsync(x => x.MapId == mapId);
        }

        public async Task<IReadOnlyList<MapRating>> GetByName(string name)
        {
            using var session = _store.QuerySession();
        
            return await session.Query<MapRating>()
                .Where(x => x.MapName.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
    }
}