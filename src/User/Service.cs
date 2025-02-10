using System.Threading.Tasks;

namespace User;

public class Service(Repository repository) {
    public async Task<User?> FindByUsername(string username) {
        return await repository.FindByUsername(username);
    }

    public async Task<User?> FindById(int id) {
        return await repository.FindById(id);
    }

    public async Task<IEnumerable<UserListItem>> FindAll() {
        return await repository.FindAll();
    }

    public async Task<User> Create(CreateUser user) {
        return await repository.Create(user);
    }
}