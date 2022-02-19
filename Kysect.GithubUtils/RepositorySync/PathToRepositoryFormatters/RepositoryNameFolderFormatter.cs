namespace Kysect.GithubUtils.RepositorySync;

public class RepositoryNameFolderProvider : IPathToRepositoryProvider
{
    private readonly string _rootPath;

    public RepositoryNameFolderProvider(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(string organization, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathToRepositoryProvider.MainDirectory, repository);
    }

    public string GetPathToRepositoryWithBranch(string organization, string branch, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathToRepositoryProvider.CustomBranchDirectory, branch, repository);
    }
}