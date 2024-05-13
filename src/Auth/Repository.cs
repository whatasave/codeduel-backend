namespace Auth;

public class Repository(Database.DatabaseContext database) {
    public void SaveRefreshToken(int userId, string refreshToken) {
        // Save refresh token to database
    }

    public void RemoveRefreshToken(int userId) {
        // Remove refresh token from database
    }
}