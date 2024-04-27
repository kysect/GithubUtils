using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.Replication.OrganizationsSync.LocalStoragePathFactories;

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

    public string GetPathToRepositoryWithBranch(GithubRepository repository, string branch)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.CustomBranchDirectory, branch, repository.Owner, repository.Name);
    }
}