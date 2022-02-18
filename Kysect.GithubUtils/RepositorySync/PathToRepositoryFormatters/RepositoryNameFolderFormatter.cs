namespace Kysect.GithubUtils;

public class RepositoryNameFolderFormatter : IPathToRepositoryFormatter
{
    private readonly string _rootPath;

    public RepositoryNameFolderFormatter(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string FormatFolderPath(string username, string repository)
    {
        return Path.Combine(_rootPath, repository);
    }
}