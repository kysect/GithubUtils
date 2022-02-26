using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.RepositorySync;

public interface IPathFormatStrategy
{
    const string MainDirectory = "repos";
    const string CustomBranchDirectory = "custom-branch";

    string GetPathToRepository(string organization, string repository);
    string GetPathToRepositoryWithBranch(string organization, string branch, string repository);
}

public static class PathToRepositoryProviderExtensions
{
    public static string GetPathToRepository(this IPathFormatStrategy pathFormatter, GithubRepository githubRepository)
    {
        ArgumentNullException.ThrowIfNull(pathFormatter);

        return pathFormatter.GetPathToRepository(githubRepository.Owner, githubRepository.Name);
    }

    public static string GetPathToRepositoryWithBranch(this IPathFormatStrategy pathFormatter, GithubRepositoryBranch repositoryBranch)
    {
        ArgumentNullException.ThrowIfNull(pathFormatter);

        return pathFormatter.GetPathToRepositoryWithBranch(repositoryBranch.Owner, repositoryBranch.Branch, repositoryBranch.Name);
    }
}