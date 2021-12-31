namespace Kysect.GithubUtils;

public interface IPathFormatter
{
    string FormatFolderPath(string username, string repository);
}