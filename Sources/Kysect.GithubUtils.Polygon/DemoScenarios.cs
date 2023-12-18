using Kysect.GithubUtils.Contributions;
using Kysect.GithubUtils.Contributions.ActivityProviders;
using Kysect.GithubUtils.Contributions.ApiResponses;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.Replication.OrganizationsSync;
using Kysect.GithubUtils.Replication.OrganizationsSync.PathProvider;
using Kysect.GithubUtils.Replication.RepositorySync;
using Kysect.GithubUtils.Replication.RepositorySync.LocalStoragePathFactories;
using Microsoft.Extensions.Logging;
using Octokit;
using System.Text.Json;

namespace Kysect.GithubUtils.Polygon;

public class DemoScenarios
{
    private readonly ILogger _logger;

    public DemoScenarios(ILogger logger)
    {
        _logger = logger;
    }

    public void CheckFetcher()
    {
        var gitUser = string.Empty;
        var token = string.Empty;
        var repositoryFetcher = new RepositoryFetcher(new RepositoryFetchOptions(gitUser, token), _logger);
        var githubRepository = new GithubRepository("kysect", "GithubUtils");
        ILocalStoragePathFactory localStoragePathFactory = new UseOwnerAndRepoForFolderNameStrategy("repo");

        repositoryFetcher.EnsureRepositoryUpdated(localStoragePathFactory, githubRepository);
        repositoryFetcher.Checkout(localStoragePathFactory, new GithubRepositoryBranch(githubRepository, "master"));
    }

    public void CheckStatParser()
    {
        using GithubActivityProvider provider = new GithubActivityProvider(_logger);
        ActivityInfo activityInfo = provider.GetActivityInfo("FrediKats");
        Console.WriteLine(JsonSerializer.Serialize(activityInfo.PerMonthActivity()));
    }

    public void CloneCustomBranch()
    {
        var gitUser = "fredikats";
        var token = string.Empty;
        var repositoryFetcher = new RepositoryFetcher(new RepositoryFetchOptions(gitUser, token), _logger);
        var organizationReplicatorPathProvider = new OrganizationReplicatorPathFormatter("test-repos");
        var organizationReplicationHub = new OrganizationReplicationHub(organizationReplicatorPathProvider, repositoryFetcher, _logger);
        organizationReplicationHub.TryAddOrganization("fredikats");
        OrganizationReplicator organizationReplicator = organizationReplicationHub.GetOrganizationReplicator("fredikats");
        organizationReplicator.Clone("fredikats");
        organizationReplicator.CloneBranch("fredikats", "master");
    }

    public void CheckOrgContributions(GitHubClient client, string organization)
    {
        var organizationContributionFetcher = new OrganizationContributionFetcher(client, _logger);
        List<OrganizationContributor> organizationContributors = organizationContributionFetcher.FetchOrganizationStatistic(organization).Result;
        organizationContributors = organizationContributors.OrderByDescending(c => c.Contributions).ToList();
        foreach ((string? username, int contributions) in organizationContributors)
        {
            Console.WriteLine($"{username}\t{contributions}");
        }
    }
}