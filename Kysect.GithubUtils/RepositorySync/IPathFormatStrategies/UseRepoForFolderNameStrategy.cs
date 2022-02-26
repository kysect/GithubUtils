namespace Kysect.GithubUtils.RepositorySync;

public class UseRepoForFolderNameStrategy : IPathFormatStrategy
{
    private readonly string _rootPath;

    public UseRepoForFolderNameStrategy(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(string organization, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathFormatStrategy.MainDirectory, repository);
    }

    public string GetPathToRepositoryWithBranch(string organization, string branch, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathFormatStrategy.CustomBranchDirectory, branch, repository);
    }
}