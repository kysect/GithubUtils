using LibGit2Sharp;

namespace Kysect.GithubUtils;

public interface IPathFormatter
{
    string FormatFolderPath(string username, string repository);
}

public class RepositoryFetcher
{
    private readonly IPathFormatter _pathFormatter;

    public RepositoryFetcher(IPathFormatter pathFormatter)
    {
        _pathFormatter = pathFormatter;
    }

    public void EnsureRepositoryUpdated(string username, string repository)
    {
        string remoteUrl = $"https://github.com/{username}/{repository}.git";
        string targetPath = _pathFormatter.FormatFolderPath(username, repository);

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
            Repository.Clone(remoteUrl, targetPath);
            return;
        }

        using var repo = new Repository(targetPath);

        Commands.Fetch(repo, remoteUrl, repo.Branches.Where(b => b.IsRemote).Select(b => b.RemoteName), new FetchOptions(), string.Empty);
    }
}