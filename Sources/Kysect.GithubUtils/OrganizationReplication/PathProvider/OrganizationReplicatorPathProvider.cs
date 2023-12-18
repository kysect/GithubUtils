using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.RepositorySync.LocalStoragePathFactories;

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

    public string GetPathToOrganizationWithBranch(string organization, string branch)
    {

        return Path.Combine(_rootDirectory, PathFormatStrategyConstant.CustomBranchDirectory, branch, organization);
    }

    public string GetPathToRepository(GithubRepository repository)
    {
        return Path.Combine(_rootDirectory, PathFormatStrategyConstant.MainDirectory, repository.Owner, repository.Name);
    }

    public string GetPathToRepositoryWithBranch(GithubRepositoryBranch repositoryBranch)
    {
        return Path.Combine(_rootDirectory, PathFormatStrategyConstant.CustomBranchDirectory, repositoryBranch.Branch, repositoryBranch.Owner, repositoryBranch.Name);
    }
}