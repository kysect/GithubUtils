using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.Replication.OrganizationsSync.PathProvider;
using Kysect.GithubUtils.Replication.RepositorySync;
using Microsoft.Extensions.Logging;

namespace Kysect.GithubUtils.Replication.OrganizationsSync;

public class OrganizationReplicator
{
    private readonly IOrganizationReplicatorPathFormatter _pathFormatter;
    private readonly string _organizationName;
    private readonly IRepositoryFetcher _repositoryFetcher;
    private readonly ILogger _logger;

    public OrganizationReplicator(IOrganizationReplicatorPathFormatter pathFormatter, string organizationName, IRepositoryFetcher repositoryFetcher, ILogger logger)
    {
        _pathFormatter = pathFormatter;
        _organizationName = organizationName;
        _repositoryFetcher = repositoryFetcher;
        _logger = logger;
    }

    public void Clone(string repository)
    {
        var githubRepository = new GithubRepository(_organizationName, repository);
        string targetPath = _pathFormatter.GetPathToRepository(githubRepository);
        _repositoryFetcher.EnsureRepositoryUpdated(targetPath, githubRepository);
    }

    public void CloneBranch(string repository, string branch)
    {
        var githubRepository = new GithubRepository(_organizationName, repository);
        string targetPath = _pathFormatter.GetPathToRepository(githubRepository);
        _repositoryFetcher.Checkout(targetPath, githubRepository, branch);
    }

    public void CloneAllBranches(string repository)
    {
        var githubRepository = new GithubRepository(_organizationName, repository);
        string originalClonedRepositoryPath = _pathFormatter.GetPathToRepository(githubRepository);

        _logger.LogDebug($"Try to clone all branches from {githubRepository} to {originalClonedRepositoryPath}");
        IReadOnlyCollection<string> branches = _repositoryFetcher.GetAllRemoteBranches(originalClonedRepositoryPath, githubRepository);

        foreach (string branch in branches)
        {
            string branchClonePath = _pathFormatter.GetPathToRepositoryWithBranch(githubRepository, branch);
            _repositoryFetcher.CloneRepositoryIfNeed(branchClonePath, githubRepository);
            _repositoryFetcher.FetchAllBranches(branchClonePath, githubRepository);
            _repositoryFetcher.CheckoutBranch(branchClonePath, githubRepository, branch);
        }
    }
}