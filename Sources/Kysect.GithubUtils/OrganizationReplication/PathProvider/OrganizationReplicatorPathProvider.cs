using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.GithubUtils.RepositorySync.IPathFormatStrategies;

namespace Kysect.GithubUtils.OrganizationReplication.PathProvider;

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

    // TODO: rework this
#pragma warning disable CA1725 // Parameter names should match base declaration
    public string GetPathToRepositoryWithBranch(string organization, string repository, string branch)
#pragma warning restore CA1725 // Parameter names should match base declaration
    {
        organization.ThrowIfNull();
        repository.ThrowIfNull();
        branch.ThrowIfNull();

        return Path.Combine(_rootDirectory, PathFormatStrategyConstant.CustomBranchDirectory, branch, organization, repository);
    }
}