namespace AuthGithub;

public class Service {
    private Repository repository;

    public Service(Repository repository) {
        this.repository = repository;
    }
    public Service(DatabaseContext database) : this(new Repository(database)) {
    }

    public Entity findById(int id) {
        return repository.findById(id);
    }
    public Entity findByProvider(int id) {
        return repository.findById(id);
    }


    public Entity authenticateWithGithub(int id) {
        return repository.findById(id);
    }
    public void GetGithubAccessToken() { }
    public void GetGithubUserData() { }
    public void GetGithubUserEmails() { }

} 