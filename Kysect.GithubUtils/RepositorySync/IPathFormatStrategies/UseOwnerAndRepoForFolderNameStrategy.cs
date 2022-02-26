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
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathFormatStrategy.MainDirectory, organization, repository);
    }

    public string GetPathToRepositoryWithBranch(string organization, string branch, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathFormatStrategy.CustomBranchDirectory, branch, organization, repository);
    }
}