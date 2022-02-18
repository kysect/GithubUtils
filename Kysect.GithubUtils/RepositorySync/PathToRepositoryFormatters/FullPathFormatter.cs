namespace Kysect.GithubUtils;

public class FullPathFormatter : IPathToRepositoryFormatter
{
    private readonly string _rootPath;

    public FullPathFormatter(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string FormatFolderPath(string username, string repository)
    {
        return Path.Combine(_rootPath, username, repository);
    }
}