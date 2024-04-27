using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.Replication.OrganizationsSync.LocalStoragePathFactories;

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

    public string GetPathToRepositoryWithBranch(GithubRepository repository, string branch)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.CustomBranchDirectory, branch, repository.Name);
    }
}