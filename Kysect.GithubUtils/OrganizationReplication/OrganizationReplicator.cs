using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.RepositorySync;

namespace Kysect.GithubUtils.OrganizationReplication;

public class OrganizationReplicator
{
    private readonly IOrganizationReplicatorPathProvider _pathProvider;
    private readonly string _organizationName;
    private readonly RepositoryFetcher _repositoryFetcher;

    public OrganizationReplicator(IOrganizationReplicatorPathProvider pathProvider, string organizationName, RepositoryFetcher repositoryFetcher)
    {
        _pathProvider = pathProvider;
        _organizationName = organizationName;
        _repositoryFetcher = repositoryFetcher;
    }

    public void Clone(string repository)
    {
        _repositoryFetcher.EnsureRepositoryUpdated(_pathProvider, new GithubRepository(_organizationName, repository));
    }

    public void CloneBranch(string repository, string branch)
    {
        _repositoryFetcher.Checkout(_pathProvider, new GithubRepositoryBranch(_organizationName, repository, branch));
    }

    public void CloneAllBranches(string repository)
    {
        _repositoryFetcher.CloneAllBranches(_pathProvider, new GithubRepository(_organizationName, repository));
    }
}