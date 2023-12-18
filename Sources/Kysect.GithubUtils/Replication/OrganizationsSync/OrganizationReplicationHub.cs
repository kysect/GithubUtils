using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.CommonLib.Collections.Extensions;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.Replication.OrganizationsSync.PathProvider;
using Kysect.GithubUtils.Replication.OrganizationsSync.RepositoryDiscovering;
using Kysect.GithubUtils.Replication.RepositorySync;
using Microsoft.Extensions.Logging;

namespace Kysect.GithubUtils.Replication.OrganizationsSync;

public class OrganizationReplicationHub
{
    private readonly IOrganizationReplicatorPathFormatter _pathFormatter;
    private readonly RepositoryFetcher _repositoryFetcher;
    private readonly ILogger _logger;

    public OrganizationReplicationHub(IOrganizationReplicatorPathFormatter pathFormatter, RepositoryFetcher repositoryFetcher, ILogger logger)
    {
        _pathFormatter = pathFormatter;
        _repositoryFetcher = repositoryFetcher;
        _logger = logger;
    }

    public bool TryAddOrganization(string organizationName)
    {
        string organizationDirectoryPath = _pathFormatter.GetPathToOrganization(organizationName);
        if (Directory.Exists(organizationDirectoryPath))
        {
            _logger.LogDebug($"Organization {organizationName} already added to hub.");
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
        string pathToOrganization = _pathFormatter.GetPathToOrganization(organizationName);
        Directory.CreateDirectory(pathToOrganization);
        return Directory
            .EnumerateDirectories(pathToOrganization)
            .Select(repositoryName => new GithubRepository(organizationName, repositoryName))
            .ToList();
    }

    public IReadOnlyCollection<GithubRepository> GetRepositories(string organizationName, string branch)
    {
        string pathToOrganization = _pathFormatter.GetPathToOrganizationWithBranch(organizationName, branch);
        Directory.CreateDirectory(pathToOrganization);
        return Directory
            .EnumerateDirectories(pathToOrganization)
            .Select(repositoryName => new GithubRepository(organizationName, repositoryName))
            .ToList();
    }

    public IReadOnlyCollection<GithubRepository> GetRepositories(string organizationName, bool useMasterBranch, params string[] branches)
    {
        organizationName.ThrowIfNull();
        branches.ThrowIfNull();

        var result = new List<GithubRepository>();
        if (useMasterBranch)
        {
            IReadOnlyCollection<GithubRepository> rootBranchRepos = GetRepositories(organizationName);
            _logger.LogDebug($"Found {rootBranchRepos.Count} repositories form root branch.");
            _logger.LogTrace($"Repositories: {rootBranchRepos.ToSingleString()}");
            result.AddRange(rootBranchRepos);
        }

        foreach (string branch in branches)
        {
            IReadOnlyCollection<GithubRepository> branchRepositories = GetRepositories(organizationName, branch);
            _logger.LogDebug($"Found {branchRepositories.Count} repositories form branch {branch}.");
            _logger.LogTrace($"Repositories: {branchRepositories.ToSingleString()}");
            result.AddRange(branchRepositories);
        }

        return result;
    }

    public void SyncOrganizations(IRepositoryDiscoveryService discoveryService)
    {
        var organizationFetcher = new OrganizationFetcher(discoveryService, _repositoryFetcher, _pathFormatter, _logger);
        IReadOnlyCollection<string> organizationNames = GetOrganizationNames();

        _logger.LogDebug($"Start organization sync. Organization count: {organizationNames.Count}");
        _logger.LogTrace($"Organization list: {organizationNames.ToSingleString()}");

        foreach (string organizationName in organizationNames)
        {
            organizationFetcher.Fetch(organizationName);
        }
    }

    public void SyncOrganizations(IRepositoryDiscoveryService discoveryService, string branch)
    {
        var organizationFetcher = new OrganizationFetcher(discoveryService, _repositoryFetcher, _pathFormatter, _logger);
        IReadOnlyCollection<string> organizationNames = GetOrganizationNames();

        _logger.LogDebug($"Start organization sync for branch {branch}. Organization count: {organizationNames.Count}");
        _logger.LogTrace($"Organization list: {organizationNames.ToSingleString()}");

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