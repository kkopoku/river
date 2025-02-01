using MongoDB.Driver;
using River.API.Models;
using River.API.Configurations;

namespace River.API.Repositories;

public class UserRepository(
     MongoDbContext mongoDbContext,
     ILogger<UserRepository> logger
) : IUserRepository
{

    private readonly IMongoCollection<User> _users = mongoDbContext.Users;
    private readonly ILogger<UserRepository> _logger = logger;

    public async Task<User> CreateUserAsync(User user)
    {

        await _users.InsertOneAsync(user);
        return user;

    }


    public async Task<User> FindUserByIdAsync(string id)
    {
        return await _users.Find(Builders<User>.Filter.Eq(w => w.Id, id)).FirstOrDefaultAsync();
    }
}