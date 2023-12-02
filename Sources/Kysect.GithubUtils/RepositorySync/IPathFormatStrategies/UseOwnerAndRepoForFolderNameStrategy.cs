namespace Kysect.GithubUtils.RepositorySync;

public class UseOwnerAndRepoForFolderNameStrategy : IPathFormatStrategy
{
    private readonly string _rootPath;

    public UseOwnerAndRepoForFolderNameStrategy(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(string organization, string repository)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.MainDirectory, organization, repository);
    }

    public string GetPathToRepositoryWithBranch(string organization, string branch, string repository)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.CustomBranchDirectory, branch, organization, repository);
    }
}