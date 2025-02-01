using River.API.Models;

namespace River.API.Repositories;

public interface IUserRepository {

    Task<User> CreateUserAsync(User user);
    Task<User> FindUserByIdAsync(string id);


}