using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.RepositorySync.LocalStoragePathFactories;

public class UseOwnerAndRepoForFolderNameStrategy : ILocalStoragePathFactory
{
    private readonly string _rootPath;

    public UseOwnerAndRepoForFolderNameStrategy(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(GithubRepository repository)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.MainDirectory, repository.Owner, repository.Name);
    }

    public string GetPathToRepositoryWithBranch(GithubRepositoryBranch repositoryBranch)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.CustomBranchDirectory, repositoryBranch.Branch, repositoryBranch.Owner, repositoryBranch.Name);
    }
}