namespace Kysect.GithubUtils.RepositorySync;

public class UseOwnerForFolderNameStrategy : IPathFormatStrategy
{
    private readonly string _rootPath;

    public UseOwnerForFolderNameStrategy(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(string organization, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathFormatStrategy.MainDirectory, organization);
    }

    public string GetPathToRepositoryWithBranch(string organization, string branch, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathFormatStrategy.CustomBranchDirectory, branch, organization);
    }
}