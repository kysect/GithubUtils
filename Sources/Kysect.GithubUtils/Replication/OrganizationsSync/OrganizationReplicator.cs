using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.Replication.OrganizationsSync.PathProvider;
using Kysect.GithubUtils.Replication.RepositorySync;

namespace Kysect.GithubUtils.Replication.OrganizationsSync;

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
        _repositoryFetcher.Checkout(_pathFormatter, new GithubRepository(_organizationName, repository), branch);
    }

    public void CloneAllBranches(string repository)
    {
        _repositoryFetcher.CloneAllBranches(_pathFormatter, new GithubRepository(_organizationName, repository));
    }
}