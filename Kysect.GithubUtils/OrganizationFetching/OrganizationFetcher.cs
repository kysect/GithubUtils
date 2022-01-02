using Kysect.GithubUtils.RepositoryDiscovering;
using Serilog;

namespace Kysect.GithubUtils.OrganizationFetching;

public class OrganizationFetcher
{
    private readonly bool _useParallelProcessing = true;

    private readonly RepositoryFetcher _repositoryFetcher;
    private readonly IRepositoryDiscoveryService _discoveryService;

    public OrganizationFetcher(IRepositoryDiscoveryService discoveryService, RepositoryFetcher repositoryFetcher)
    {
        _discoveryService = discoveryService;
        _repositoryFetcher = repositoryFetcher;
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

    private GithubOrganizationRepository SyncRepository(RepositoryRecord repository, string organizationName, string? branch = null)
    {
        string path = _repositoryFetcher.EnsureRepositoryUpdated(organizationName, repository.Name);
        if (branch is not null)
            _repositoryFetcher.Checkout(organizationName, repository.Name, branch);

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