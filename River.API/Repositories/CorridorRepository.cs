using MongoDB.Driver;
using River.API.Models;
using River.API.Configurations;
using Microsoft.Extensions.Logging;

namespace River.API.Repositories;

public class CorridorRepository(
    MongoDbContext mongoDbContext,
    ILogger<CorridorRepository> logger
) : ICorridorRepository
{
    private readonly IMongoCollection<Corridor> _corridors = mongoDbContext.Corridors;
    private readonly ILogger<CorridorRepository> _logger = logger;

    public async Task<Corridor> CreateCorridorAsync(Corridor corridor)
    {
        await _corridors.InsertOneAsync(corridor);
        return corridor;
    }

    public async Task<Corridor?> FindCorridorByIdAsync(string id)
    {
        return await _corridors.Find(Builders<Corridor>.Filter.Eq(c => c.Id, id)).FirstOrDefaultAsync();
    }

    public async Task<List<Corridor>> GetAllCorridorsAsync()
    {
        return await _corridors.Find(_ => true).ToListAsync();
    }
}

