using Kysect.GithubUtils.RepositorySync;
using Kysect.GithubUtils.RepositorySync.Models;

namespace Kysect.GithubUtils.OrganizationReplication;

public class OrganizationReplicator
{
    private readonly IOrganizationReplicatorPathFormatter _pathFormatter;
    private readonly string _organizationName;
    private readonly RepositoryFetcher _repositoryFetcher;

    public OrganizationReplicator(IOrganizationReplicatorPathFormatter pathFormatter, string organizationName, RepositoryFetcher repositoryFetcher)
    {
        _pathFormatter = pathFormatter;
        _organizationName = organizationName;
        _repositoryFetcher = repositoryFetcher;
    }

    public void Clone(string repository)
    {
        _repositoryFetcher.EnsureRepositoryUpdated(_pathFormatter, new GithubRepository(_organizationName, repository));
    }

    public void CloneBranch(string repository, string branch)
    {
        _repositoryFetcher.Checkout(_pathFormatter, new GithubRepositoryBranch(_organizationName, repository, branch));
    }

    public void CloneAllBranches(string repository)
    {
        _repositoryFetcher.CloneAllBranches(_pathFormatter, new GithubRepository(_organizationName, repository));
    }
}