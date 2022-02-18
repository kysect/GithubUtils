using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.RepositorySync;

public interface IPathToRepositoryFormatter
{
    string FormatFolderPath(GithubRepository githubRepository);
}