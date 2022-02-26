using Kysect.CommonLib;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.RepositoryDiscovering;
using Kysect.GithubUtils.RepositorySync;
using Serilog;

namespace Kysect.GithubUtils.OrganizationReplication;

public class OrganizationReplicationHub
{
    private readonly IOrganizationReplicatorPathFormatter _pathFormatter;
    private readonly RepositoryFetcher _repositoryFetcher;

    public OrganizationReplicationHub(IOrganizationReplicatorPathFormatter pathFormatter, RepositoryFetcher repositoryFetcher)
    {
        ArgumentNullException.ThrowIfNull(pathFormatter);

        _pathFormatter = pathFormatter;
        _repositoryFetcher = repositoryFetcher;
    }

    public bool TryAddOrganization(string organizationName)
    {
        ArgumentNullException.ThrowIfNull(organizationName);

        string organizationDirectoryPath = _pathFormatter.GetPathToOrganization(organizationName);
        if (Directory.Exists(organizationDirectoryPath))
        {
            Log.Debug($"Organization {organizationName} already added to hub.");
            return false;
        }

        Directory.CreateDirectory(organizationDirectoryPath);
        return true;
    }

    public IReadOnlyCollection<string> GetOrganizationNames()
    {
        string pathToOrganizations = _pathFormatter.GetPathToOrganizations();
        Directory.CreateDirectory(pathToOrganizations);
        return Directory.EnumerateDirectories(pathToOrganizations).ToList();
    }

    public IReadOnlyCollection<GithubRepository> GetRepositories(string organizationName)
    {
        ArgumentNullException.ThrowIfNull(organizationName);

        string pathToOrganization = _pathFormatter.GetPathToOrganization(organizationName);
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

        string pathToOrganization = _pathFormatter.GetPathToOrganizationWithBranch(organizationName, branch);
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
        {
            IReadOnlyCollection<GithubRepository> rootBranchRepos = GetRepositories(organizationName);
            Log.Debug($"Found {rootBranchRepos.Count} repositories form root branch.");
            Log.Verbose($"Repositories: {rootBranchRepos.ToSingleString()}");
            result.AddRange(rootBranchRepos);
        }

        foreach (string branch in branches)
        {
            IReadOnlyCollection<GithubRepository> branchRepositories = GetRepositories(organizationName, branch);
            Log.Debug($"Found {branchRepositories.Count} repositories form branch {branch}.");
            Log.Verbose($"Repositories: {branchRepositories.ToSingleString()}");
            result.AddRange(branchRepositories);
        }

        return result;
    }

    public void SyncOrganizations(IRepositoryDiscoveryService discoveryService)
    {
        var organizationFetcher = new OrganizationFetcher(discoveryService, _repositoryFetcher, _pathFormatter);
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
        var organizationFetcher = new OrganizationFetcher(discoveryService, _repositoryFetcher, _pathFormatter);
        IReadOnlyCollection<string> organizationNames = GetOrganizationNames();

        Log.Debug($"Start organization sync for branch {branch}. Organization count: {organizationNames.Count}");
        Log.Verbose($"Organization list: {organizationNames.ToSingleString()}");

        foreach (string organizationName in organizationNames)
        {
            organizationFetcher.Fetch(organizationName, branch);
        }
    }

    public OrganizationReplicator GetOrganizationReplicator(string repository)
    {
        return new OrganizationReplicator(_pathFormatter, repository, _repositoryFetcher);
    }
}