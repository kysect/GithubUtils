using Kysect.CommonLib;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.RepositoryDiscovering;
using Kysect.GithubUtils.RepositorySync;
using Serilog;

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

    public IReadOnlyCollection<GithubRepository> GetRepositories(string organizationName, bool useMasterBranch, params string[] branches)
    {
        ArgumentNullException.ThrowIfNull(organizationName);
        ArgumentNullException.ThrowIfNull(branches);

        var result = new List<GithubRepository>();
        if (useMasterBranch)
            result.AddRange(GetRepositories(organizationName));

        foreach (string branch in branches)
            result.AddRange(GetRepositories(organizationName, branch));

        return result;
    }

    public void SyncOrganizations(IRepositoryDiscoveryService discoveryService)
    {
        var organizationFetcher = new OrganizationFetcher(discoveryService, _repositoryFetcher, _pathProvider);
        IReadOnlyCollection<string> organizationNames = GetOrganizationNames();

        Log.Debug($"Start organization sync. Organization count: {organizationNames.Count}");
        Log.Verbose($"Organization list: {organizationNames.ToSingleString()}");

        foreach (string organizationName in organizationNames)
        {
            organizationFetcher.Fetch(organizationName);
        }
    }

    public void SyncOrganizations(IRepositoryDiscoveryService discoveryService, string branch)
    {
        var organizationFetcher = new OrganizationFetcher(discoveryService, _repositoryFetcher, _pathProvider);
        IReadOnlyCollection<string> organizationNames = GetOrganizationNames();

        Log.Debug($"Start organization sync. Organization count: {organizationNames.Count}");
        Log.Verbose($"Organization list: {organizationNames.ToSingleString()}");

        foreach (string organizationName in organizationNames)
        {
            organizationFetcher.Fetch(organizationName, branch);
        }
    }

    public OrganizationReplicator GetOrganizationReplicator(string repository)
    {
        return new OrganizationReplicator(_pathProvider, repository, _repositoryFetcher);
    }
}