using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.RepositoryDiscovering;
using Microsoft.Extensions.Logging;

namespace Kysect.GithubUtils.RepositorySync;

public class OrganizationFetcher
{
    private readonly bool _useParallelProcessing;

    private readonly RepositoryFetcher _repositoryFetcher;
    private readonly IPathFormatStrategy _pathFormatter;
    private readonly IRepositoryDiscoveryService _discoveryService;
    private readonly ILogger _logger;

    public OrganizationFetcher(IRepositoryDiscoveryService discoveryService, RepositoryFetcher repositoryFetcher, IPathFormatStrategy pathFormatter, ILogger logger, bool useParallelProcessing = true)
    {
        _discoveryService = discoveryService;
        _repositoryFetcher = repositoryFetcher;
        _pathFormatter = pathFormatter;
        _logger = logger;
        _useParallelProcessing = useParallelProcessing;
    }

    public IReadOnlyCollection<GithubOrganizationRepository> Fetch(string organizationName, string? branch = null)
    {
        _logger.LogInformation($"Start discovering repositories from {organizationName}");
        List<RepositoryRecord> repositoryRecords = GetRepositoryList(_discoveryService, organizationName).Result;
        _logger.LogInformation($"Discovered {repositoryRecords.Count} repositories");

        if (_useParallelProcessing)
        {
            _logger.LogInformation("Start parallel processing");

            List<GithubOrganizationRepository> result = repositoryRecords
                .AsParallel()
                .Select(r => SyncRepository(r, organizationName, branch))
                .ToList();

            return result;
        }
        else
        {
            _logger.LogInformation("Start single thread processing");

            List<GithubOrganizationRepository> result = repositoryRecords
                .Select(r => SyncRepository(r, organizationName, branch))
                .ToList();

            return result;
        }
    }

    private GithubOrganizationRepository SyncRepository(RepositoryRecord repository, string organizationName, string? branch)
    {
        var githubRepository = new GithubRepository(organizationName, repository.Name);
        string path = _repositoryFetcher.EnsureRepositoryUpdated(_pathFormatter, githubRepository);

        if (branch is not null)
        {
            var githubRepositoryBranch = new GithubRepositoryBranch(organizationName, repository.Name, branch);
            _repositoryFetcher.Checkout(_pathFormatter, githubRepositoryBranch);
        }

        return new GithubOrganizationRepository(path, organizationName, repository.Name);
    }

    private async Task<List<RepositoryRecord>> GetRepositoryList(IRepositoryDiscoveryService discoveryService, string organizationName)
    {
        var repos = new List<RepositoryRecord>();
        await foreach (RepositoryRecord repositoryRecord in discoveryService.TryDiscover(organizationName))
            repos.Add(repositoryRecord);
        return repos;
    }
}