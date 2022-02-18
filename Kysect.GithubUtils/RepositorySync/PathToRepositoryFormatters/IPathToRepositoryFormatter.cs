namespace Kysect.GithubUtils.RepositorySync;

public interface IPathToRepositoryFormatter
{
    string FormatFolderPath(string username, string repository);
}