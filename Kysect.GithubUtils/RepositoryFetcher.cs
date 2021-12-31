using LibGit2Sharp;

namespace Kysect.GithubUtils;

public interface IPathFormatter
{
    string FormatFolderPath(string username, string repository);
}

public class RepositoryFetcher
{
    private readonly IPathFormatter _pathFormatter;
    private readonly string _gitUser;
    private readonly string _token;

    public RepositoryFetcher(IPathFormatter pathFormatter, string gitUser, string token)
    {
        _pathFormatter = pathFormatter;
        _gitUser = gitUser;
        _token = token;
    }

    public void EnsureRepositoryUpdated(string username, string repository)
    {
        string remoteUrl = $"https://github.com/{username}/{repository}.git";
        string targetPath = _pathFormatter.FormatFolderPath(username, repository);

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = CreateCredentialsProvider
            };
            Repository.Clone(remoteUrl, targetPath, cloneOptions);
            return;
        }

        using var repo = new Repository(targetPath);

        var fetchOptions = new FetchOptions
        {
            CredentialsProvider = CreateCredentialsProvider
        };
        Remote remote = repo.Network.Remotes["origin"];
        IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
        Commands.Fetch(repo, remote.Name, refSpecs, fetchOptions, string.Empty);
    }

    private UsernamePasswordCredentials CreateCredentialsProvider(string url, string usernameFromUrl, SupportedCredentialTypes types)
    {
        return new UsernamePasswordCredentials { Username = _gitUser, Password = _token };
    }
}