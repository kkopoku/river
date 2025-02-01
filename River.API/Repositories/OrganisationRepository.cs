using MongoDB.Driver;
using River.API.DTOs;
using River.API.Models;
using River.API.Configurations;

namespace River.API.Repositories
{
    public class OrganisationRepository(
        MongoDbContext mongoDbContext
    ) : IOrganisationRepository
    {
        private readonly IMongoCollection<Organisation> _organisations = mongoDbContext.Organisations;

        public async Task<Organisation> CreateOrganisationAsync(Organisation organisation)
        {
            await _organisations.InsertOneAsync(organisation);
            return organisation;
        }

        public async Task<Organisation> FindOrganisationByIdAsync(string id)
        {
            return await _organisations.Find(Builders<Organisation>.Filter.Eq(o => o.Id, id)).FirstOrDefaultAsync();
        }
    }
}
