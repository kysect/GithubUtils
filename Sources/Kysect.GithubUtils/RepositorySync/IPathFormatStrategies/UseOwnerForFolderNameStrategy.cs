namespace Kysect.GithubUtils.RepositorySync.IPathFormatStrategies;

public class UseOwnerForFolderNameStrategy : IPathFormatStrategy
{
    private readonly string _rootPath;

    public UseOwnerForFolderNameStrategy(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(string organization, string repository)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.MainDirectory, organization);
    }

    public string GetPathToRepositoryWithBranch(string organization, string branch, string repository)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.CustomBranchDirectory, branch, organization);
    }
}