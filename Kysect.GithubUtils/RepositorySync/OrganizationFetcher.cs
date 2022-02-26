using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.RepositoryDiscovering;
using Serilog;

namespace Kysect.GithubUtils.RepositorySync;

public class OrganizationFetcher
{
    private readonly bool _useParallelProcessing;

    private readonly RepositoryFetcher _repositoryFetcher;
    private readonly IPathFormatStrategy _pathFormatter;
    private readonly IRepositoryDiscoveryService _discoveryService;

    public OrganizationFetcher(IRepositoryDiscoveryService discoveryService, RepositoryFetcher repositoryFetcher, IPathFormatStrategy pathFormatter, bool useParallelProcessing = true)
    {
        _discoveryService = discoveryService;
        _repositoryFetcher = repositoryFetcher;
        _pathFormatter = pathFormatter;
        _useParallelProcessing = useParallelProcessing;
    }

    public IReadOnlyCollection<GithubOrganizationRepository> Fetch(string organizationName, string? branch = null)
    {
        Log.Information($"Start discovering repositories from {organizationName}");
        List<RepositoryRecord> repositoryRecords = GetRepositoryList(_discoveryService, organizationName).Result;
        Log.Information($"Discovered {repositoryRecords.Count} repositories");

        if (_useParallelProcessing)
        {
            Log.Information("Start parallel processing");

            List<GithubOrganizationRepository> result = repositoryRecords
                .AsParallel()
                .Select(r => SyncRepository(r, organizationName, branch))
                .ToList();

            return result;
        }
        else
        {
            Log.Information("Start single thread processing");

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