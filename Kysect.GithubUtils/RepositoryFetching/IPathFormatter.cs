namespace Kysect.GithubUtils;

public interface IPathFormatter
{
    string FormatFolderPath(string username, string repository);
}

public class FullPathFormatter : IPathFormatter
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

public class UserNameFolderFormatter : IPathFormatter
{
    private readonly string _rootPath;

    public UserNameFolderFormatter(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string FormatFolderPath(string username, string repository)
    {
        return Path.Combine(_rootPath, username);
    }
}

public class RepositoryNameFolderFormatter : IPathFormatter
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