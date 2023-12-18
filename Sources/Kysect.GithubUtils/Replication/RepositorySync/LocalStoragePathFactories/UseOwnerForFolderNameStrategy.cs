using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.Replication.RepositorySync.LocalStoragePathFactories;

public class UseOwnerForFolderNameStrategy : ILocalStoragePathFactory
{
    private readonly string _rootPath;

    public UseOwnerForFolderNameStrategy(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(GithubRepository repository)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.MainDirectory, repository.Owner);
    }

    public string GetPathToRepositoryWithBranch(GithubRepositoryBranch repositoryBranch)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.CustomBranchDirectory, repositoryBranch.Branch, repositoryBranch.Owner);
    }
}