using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.RepositorySync;

public interface IPathToRepositoryProvider
{
    const string MainDirectory = "repos";
    const string CustomBranchDirectory = "custom-branch";

    string GetPathToRepository(string organization, string repository);
    string GetPathToRepositoryWithBranch(string organization, string branch, string repository);
}

public static class PathToRepositoryProviderExtensions
{
    public static string GetPathToRepository(this IPathToRepositoryProvider pathProvider, GithubRepository githubRepository)
    {
        ArgumentNullException.ThrowIfNull(pathProvider);

        return pathProvider.GetPathToRepository(githubRepository.Owner, githubRepository.Name);
    }

    public static string GetPathToRepositoryWithBranch(this IPathToRepositoryProvider pathProvider, GithubRepository githubRepository, string branch)
    {
        ArgumentNullException.ThrowIfNull(pathProvider);
        ArgumentNullException.ThrowIfNull(branch);

        return pathProvider.GetPathToRepositoryWithBranch(githubRepository.Owner, branch, githubRepository.Name);
    }
}