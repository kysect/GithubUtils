namespace Kysect.GithubUtils;

public interface IPathToRepositoryFormatter
{
    string FormatFolderPath(string username, string repository);
}