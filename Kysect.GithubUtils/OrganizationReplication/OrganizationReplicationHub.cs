using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.RepositoryDiscovering;
using Kysect.GithubUtils.RepositorySync;

namespace Kysect.GithubUtils.OrganizationReplication;

public class OrganizationReplicationHub
{
    private readonly IOrganizationReplicatorPathProvider _pathProvider;
    private readonly RepositoryFetcher _repositoryFetcher;

    public OrganizationReplicationHub(IOrganizationReplicatorPathProvider pathProvider, RepositoryFetcher repositoryFetcher)
    {
        ArgumentNullException.ThrowIfNull(pathProvider);

        _pathProvider = pathProvider;
        _repositoryFetcher = repositoryFetcher;
    }

    public bool TryAddOrganization(string organizationName)
    {
        ArgumentNullException.ThrowIfNull(organizationName);

        string organizationDirectoryPath = _pathProvider.GetPathToOrganization(organizationName);
        if (Directory.Exists(organizationDirectoryPath))
            return false;

        Directory.CreateDirectory(organizationDirectoryPath);
        return true;
    }

    public IReadOnlyCollection<string> GetOrganizationNames()
    {
        string pathToOrganizations = _pathProvider.GetPathToOrganizations();
        Directory.CreateDirectory(pathToOrganizations);
        return Directory.EnumerateDirectories(pathToOrganizations).ToList();
    }

    public IReadOnlyCollection<GithubRepository> GetRepositories(string organizationName)
    {
        ArgumentNullException.ThrowIfNull(organizationName);

        string pathToOrganization = _pathProvider.GetPathToOrganization(organizationName);
        Directory.CreateDirectory(pathToOrganization);
        return Directory
            .EnumerateDirectories(pathToOrganization)
            .Select(repositoryName => new GithubRepository(organizationName, repositoryName))
            .ToList();
    }

    public IReadOnlyCollection<GithubRepository> GetRepositories(string organizationName, string branch)
    {
        ArgumentNullException.ThrowIfNull(organizationName);
        ArgumentNullException.ThrowIfNull(branch);

        string pathToOrganization = _pathProvider.GetPathToOrganizationWithBranch(organizationName, branch);
        Directory.CreateDirectory(pathToOrganization);
        return Directory
            .EnumerateDirectories(pathToOrganization)
            .Select(repositoryName => new GithubRepository(organizationName, repositoryName))
            .ToList();
    }

    public void SyncOrganizations(IRepositoryDiscoveryService discoveryService)
    {
        var organizationFetcher = new OrganizationFetcher(discoveryService, _repositoryFetcher, _pathProvider);

        foreach (string organizationName in GetOrganizationNames())
        {
            organizationFetcher.Fetch(organizationName);
        }
    }

    public OrganizationReplicator GetOrganizationReplicator(string repository)
    {
        return new OrganizationReplicator(_pathProvider, repository, _repositoryFetcher);
    }
}