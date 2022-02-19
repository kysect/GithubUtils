namespace Kysect.GithubUtils.RepositorySync;

public class OwnerFolderProvider : IPathToRepositoryProvider
{
    private readonly string _rootPath;

    public OwnerFolderProvider(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(string organization, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathToRepositoryProvider.MainDirectory, organization);
    }

    public string GetPathToRepositoryWithBranch(string organization, string branch, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathToRepositoryProvider.CustomBranchDirectory, branch, organization);
    }
}