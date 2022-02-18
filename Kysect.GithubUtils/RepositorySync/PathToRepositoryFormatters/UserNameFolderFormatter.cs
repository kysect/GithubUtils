using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.RepositorySync;

public class OwnerFolderFormatter : IPathToRepositoryFormatter
{
    private readonly string _rootPath;

    public OwnerFolderFormatter(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string FormatFolderPath(GithubRepository githubRepository)
    {
        return Path.Combine(_rootPath, githubRepository.Owner);
    }
}