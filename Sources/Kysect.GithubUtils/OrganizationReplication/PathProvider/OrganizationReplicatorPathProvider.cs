using Kysect.GithubUtils.RepositorySync;

namespace Kysect.GithubUtils.OrganizationReplication;

public class OrganizationReplicatorPathFormatter : IOrganizationReplicatorPathFormatter
{
    private readonly string _rootDirectory;

    public OrganizationReplicatorPathFormatter(string rootDirectory)
    {
        _rootDirectory = rootDirectory;
    }

    public string GetPathToOrganizations()
    {
        return Path.Combine(_rootDirectory, PathFormatStrategyConstant.MainDirectory);
    }

    public string GetPathToOrganization(string organization)
    {
        return Path.Combine(_rootDirectory, PathFormatStrategyConstant.MainDirectory, organization);
    }

    public string GetPathToRepository(string organization, string repository)
    {
        return Path.Combine(_rootDirectory, PathFormatStrategyConstant.MainDirectory, organization, repository);
    }

    public string GetPathToOrganizationWithBranch(string organization, string branch)
    {

        return Path.Combine(_rootDirectory, PathFormatStrategyConstant.CustomBranchDirectory, branch, organization);
    }

    public string GetPathToRepositoryWithBranch(string organization, string repository, string branch)
    {

        return Path.Combine(_rootDirectory, PathFormatStrategyConstant.CustomBranchDirectory, branch, organization, repository);
    }
}