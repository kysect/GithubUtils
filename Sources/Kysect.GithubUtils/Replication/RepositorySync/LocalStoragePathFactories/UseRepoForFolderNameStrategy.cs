using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.Replication.RepositorySync.LocalStoragePathFactories;

public class UseRepoForFolderNameStrategy : ILocalStoragePathFactory
{
    private readonly string _rootPath;

    public UseRepoForFolderNameStrategy(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(GithubRepository repository)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.MainDirectory, repository.Name);
    }

    public string GetPathToRepositoryWithBranch(GithubRepositoryBranch repositoryBranch)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.CustomBranchDirectory, repositoryBranch.Branch, repositoryBranch.Name);
    }
}