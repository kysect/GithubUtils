namespace Kysect.GithubUtils.RepositorySync;

public class FullPathProvider : IPathToRepositoryProvider
{
    private readonly string _rootPath;

    public FullPathProvider(string rootPath)
    {
        _rootPath = rootPath;
    }

    public string GetPathToRepository(string organization, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathToRepositoryProvider.MainDirectory, organization, repository);
    }

    public string GetPathToRepositoryWithBranch(string organization, string branch, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootPath, IPathToRepositoryProvider.CustomBranchDirectory, branch, organization, repository);
    }
}