using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace User;

public class Repository(Database.DatabaseContext database) {
    public async Task<User?> FindById(int id) {
        Console.WriteLine($"[User Repository] Getting user by id {id}");
        return await (from user in database.Users where user.Id == id select new User(user)).SingleOrDefaultAsync();
    }

    public async Task<User?> FindByUsername(string username) {
        Console.WriteLine($"[User Repository] Getting user by username {username}");
        return await (from user in database.Users where user.Username == username select new User(user)).SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<UserListItem>> FindAll() {
        Console.WriteLine("[User Repository] Getting all users");
        return await (from user in database.Users select new UserListItem(user)).ToListAsync();
    }

    public async Task<User> Create(CreateUser user) {
        Console.WriteLine($"[User Repository] Creating user with username {user.Username}");
        var entry = await database.Users.AddAsync(new Entity {
            Username = user.Username,
            Name = user.Name,
            Avatar = user.Avatar,
            BackgroundImage = user.BackgroundImage,
            Biography = user.Biography
        });
        await database.SaveChangesAsync();
        return new User(entry.Entity);
    }

    public async Task DeleteByIdAsync(int id) {
        Console.WriteLine($"[User Repository] Deleting user by id {id}");
        var user = await database.Users.FindAsync(id) ?? throw new Exception("User not found");

        user.Username = $"deleted-{id}";
        user.Name = $"deleted-{id}";
        user.Avatar = null;
        user.BackgroundImage = null;
        user.Biography = null;

        await database.SaveChangesAsync();
    }
}