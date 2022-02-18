using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.RepositorySync;

public class FullPathFormatter : IPathToRepositoryFormatter
{
    private readonly string _rootPath;

    public FullPathFormatter(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string FormatFolderPath(GithubRepository githubRepository)
    {
        return Path.Combine(_rootPath, githubRepository.Owner, githubRepository.Name);
    }
}