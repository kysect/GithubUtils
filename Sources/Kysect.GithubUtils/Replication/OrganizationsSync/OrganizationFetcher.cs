using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.Replication.OrganizationsSync.RepositoryDiscovering;
using Kysect.GithubUtils.Replication.RepositorySync;
using Kysect.GithubUtils.Replication.RepositorySync.LocalStoragePathFactories;
using Microsoft.Extensions.Logging;

namespace Kysect.GithubUtils.Replication.OrganizationsSync;

public class OrganizationFetcher
{
    private readonly bool _useParallelProcessing;

    private readonly RepositoryFetcher _repositoryFetcher;
    private readonly ILocalStoragePathFactory _pathFormatter;
    private readonly IRepositoryDiscoveryService _discoveryService;
    private readonly ILogger _logger;

    public OrganizationFetcher(IRepositoryDiscoveryService discoveryService, RepositoryFetcher repositoryFetcher, ILocalStoragePathFactory pathFormatter, ILogger logger, bool useParallelProcessing = true)
    {
        _discoveryService = discoveryService;
        _repositoryFetcher = repositoryFetcher;
        _pathFormatter = pathFormatter;
        _logger = logger;
        _useParallelProcessing = useParallelProcessing;
    }

    public IReadOnlyCollection<ClonedGithubRepository> Fetch(string organizationName, string? branch = null)
    {
        _logger.LogInformation($"Start discovering repositories from {organizationName}");
        IReadOnlyCollection<GithubRepository> repositoryRecords = _discoveryService.GetRepositories(organizationName).Result;
        _logger.LogInformation($"Discovered {repositoryRecords.Count} repositories");

        if (_useParallelProcessing)
        {
            _logger.LogInformation("Start parallel processing");

            var result = repositoryRecords
                .AsParallel()
                .Select(r => SyncRepository(r, branch))
                .ToList();

            return result;
        }
        else
        {
            _logger.LogInformation("Start single thread processing");

            List<ClonedGithubRepository> result = repositoryRecords
                .Select(r => SyncRepository(r, branch))
                .ToList();

            return result;
        }
    }

    private ClonedGithubRepository SyncRepository(GithubRepository githubRepository, string? branch)
    {
        string path = _repositoryFetcher.EnsureRepositoryUpdated(_pathFormatter, githubRepository);

        if (branch is not null)
        {
            var githubRepositoryBranch = new GithubRepositoryBranch(githubRepository.Owner, githubRepository.Name, branch);
            _repositoryFetcher.Checkout(_pathFormatter, githubRepositoryBranch);
        }

        return new ClonedGithubRepository(path, githubRepository.Owner, githubRepository.Name);
    }
}