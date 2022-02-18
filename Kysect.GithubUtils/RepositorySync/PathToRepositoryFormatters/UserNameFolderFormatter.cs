namespace Kysect.GithubUtils.RepositorySync;

public class UserNameFolderFormatter : IPathToRepositoryFormatter
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