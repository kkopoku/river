using River.API.Models;

namespace River.API.Repositories;

public interface ICorridorRepository
{
    Task<Corridor> CreateCorridorAsync(Corridor corridor);
    Task<Corridor?> FindCorridorByIdAsync(string id);
    Task<List<Corridor>> GetAllCorridorsAsync();
}

