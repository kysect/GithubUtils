using Kysect.GithubUtils.RepositorySync;

namespace Kysect.GithubUtils.OrganizationReplication;

public class OrganizationReplicatorPathProvider : IOrganizationReplicatorPathProvider
{
    

    private readonly string _rootDirectory;

    public OrganizationReplicatorPathProvider(string rootDirectory)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);

        _rootDirectory = rootDirectory;
    }

    public string GetPathToOrganizations()
    {
        return Path.Combine(_rootDirectory, IPathToRepositoryProvider.MainDirectory);
    }

    public string GetPathToOrganization(string organization)
    {
        ArgumentNullException.ThrowIfNull(organization);

        return Path.Combine(_rootDirectory, IPathToRepositoryProvider.MainDirectory, organization);
    }

    public string GetPathToRepository(string organization, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootDirectory, IPathToRepositoryProvider.MainDirectory, organization, repository);
    }

    public string GetPathToOrganizationWithBranch(string organization, string branch)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(branch);

        return Path.Combine(_rootDirectory, IPathToRepositoryProvider.CustomBranchDirectory, branch, organization);
    }

    public string GetPathToRepositoryWithBranch(string organization, string repository, string branch)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(branch);

        return Path.Combine(_rootDirectory, IPathToRepositoryProvider.CustomBranchDirectory, branch, organization, repository);
    }
}