namespace Kysect.GithubUtils.RepositorySync.IPathFormatStrategies;

public class UseRepoForFolderNameStrategy : IPathFormatStrategy
{
    private readonly string _rootPath;

    public UseRepoForFolderNameStrategy(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(string organization, string repository)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.MainDirectory, repository);
    }

    public string GetPathToRepositoryWithBranch(string organization, string branch, string repository)
    {
        return Path.Combine(_rootPath, PathFormatStrategyConstant.CustomBranchDirectory, branch, repository);
    }
}