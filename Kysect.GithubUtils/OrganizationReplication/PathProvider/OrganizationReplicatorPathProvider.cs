using Kysect.GithubUtils.RepositorySync;

namespace Kysect.GithubUtils.OrganizationReplication;

public class OrganizationReplicatorPathFormatter : IOrganizationReplicatorPathFormatter
{
    private readonly string _rootDirectory;

    public OrganizationReplicatorPathFormatter(string rootDirectory)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);

        _rootDirectory = rootDirectory;
    }

    public string GetPathToOrganizations()
    {
        return Path.Combine(_rootDirectory, IPathFormatStrategy.MainDirectory);
    }

    public string GetPathToOrganization(string organization)
    {
        ArgumentNullException.ThrowIfNull(organization);

        return Path.Combine(_rootDirectory, IPathFormatStrategy.MainDirectory, organization);
    }

    public string GetPathToRepository(string organization, string repository)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);

        return Path.Combine(_rootDirectory, IPathFormatStrategy.MainDirectory, organization, repository);
    }

    public string GetPathToOrganizationWithBranch(string organization, string branch)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(branch);

        return Path.Combine(_rootDirectory, IPathFormatStrategy.CustomBranchDirectory, branch, organization);
    }

    public string GetPathToRepositoryWithBranch(string organization, string repository, string branch)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(branch);

        return Path.Combine(_rootDirectory, IPathFormatStrategy.CustomBranchDirectory, branch, organization, repository);
    }
}