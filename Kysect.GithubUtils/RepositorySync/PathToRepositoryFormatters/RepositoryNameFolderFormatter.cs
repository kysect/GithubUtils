using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.RepositorySync;

public class RepositoryNameFolderFormatter : IPathToRepositoryFormatter
{
    private readonly string _rootPath;

    public RepositoryNameFolderFormatter(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string FormatFolderPath(GithubRepository githubRepository)
    {
        return Path.Combine(_rootPath, githubRepository.Name);
    }
}