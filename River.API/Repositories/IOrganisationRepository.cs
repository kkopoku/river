using River.API.Models;

namespace River.API.Repositories;

public interface IOrganisationRepository {

    Task<Organisation> CreateOrganisationAsync(Organisation organisation);
    Task<Organisation> FindOrganisationByIdAsync(string id);

}