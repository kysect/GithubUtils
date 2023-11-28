using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.GithubUtils.RepositorySync.Models;

namespace Kysect.GithubUtils.RepositorySync;

public static class PathFormatStrategyConstant
{
    public const string MainDirectory = "repos";
    public const string CustomBranchDirectory = "custom-branch";
}

public interface IPathFormatStrategy
{
    string GetPathToRepository(string organization, string repository);
    string GetPathToRepositoryWithBranch(string organization, string branch, string repository);
}

public static class PathToRepositoryProviderExtensions
{
    public static string GetPathToRepository(this IPathFormatStrategy pathFormatter, GithubRepository githubRepository)
    {
        pathFormatter.ThrowIfNull();

        return pathFormatter.GetPathToRepository(githubRepository.Owner, githubRepository.Name);
    }

    public static string GetPathToRepositoryWithBranch(this IPathFormatStrategy pathFormatter, GithubRepositoryBranch repositoryBranch)
    {
        pathFormatter.ThrowIfNull();

        return pathFormatter.GetPathToRepositoryWithBranch(repositoryBranch.Owner, repositoryBranch.Branch, repositoryBranch.Name);
    }
}