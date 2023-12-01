using System.Text.Json;
using Kysect.CommonLib.DependencyInjection;
using Kysect.GithubUtils.Contributions;
using Kysect.GithubUtils.OrganizationReplication;
using Kysect.GithubUtils.RepositorySync;
using Kysect.GithubUtils.RepositorySync.Models;
using Octokit;

var logger = PredefinedLogger.CreateConsoleLogger();

string token = String.Empty;

var client = new GitHubClient(new ProductHeaderValue("Kysect"))
{
    Credentials = new Credentials(token)
};

CheckOrgContributions(client, "kysect");

void CheckFetcher()
{
    var gitUser = string.Empty;
    var token = string.Empty;
    var repositoryFetcher = new RepositoryFetcher(new RepositoryFetchOptions(gitUser, token), logger);
    var githubRepository = new GithubRepository("fredikats", "test");
    repositoryFetcher.EnsureRepositoryUpdated(new UseOwnerAndRepoForFolderNameStrategy("repo"), githubRepository);
    repositoryFetcher.Checkout(new UseOwnerAndRepoForFolderNameStrategy("repo"), new GithubRepositoryBranch(githubRepository, "main"));
    repositoryFetcher.Checkout(new UseOwnerAndRepoForFolderNameStrategy("repo"), new GithubRepositoryBranch(githubRepository, "qq"));
}

void CheckStatParser()
{
    IGithubActivityProvider provider = new GithubActivityProvider(logger);
    ActivityInfo activityInfo = provider.GetActivityInfo("FrediKats");
    Console.WriteLine(JsonSerializer.Serialize(activityInfo.PerMonthActivity()));
}

void CloneCustomBranch()
{
    var gitUser = "fredikats";
    var token = string.Empty;
    var repositoryFetcher = new RepositoryFetcher(new RepositoryFetchOptions(gitUser, token), logger);
    var organizationReplicatorPathProvider = new OrganizationReplicatorPathFormatter("test-repos");
    var organizationReplicationHub = new OrganizationReplicationHub(organizationReplicatorPathProvider, repositoryFetcher, logger);
    organizationReplicationHub.TryAddOrganization("fredikats");
    OrganizationReplicator organizationReplicator = organizationReplicationHub.GetOrganizationReplicator("fredikats");
    organizationReplicator.Clone("fredikats");
    organizationReplicator.CloneBranch("fredikats", "master");
}

void CheckOrgContributions(GitHubClient client, string organization)
{
    var organizationContributionFetcher = new OrganizationContributionFetcher(client, logger);
    List<OrganizationContributor> organizationContributors = organizationContributionFetcher.FetchOrganizationStatistic(organization).Result;
    organizationContributors = organizationContributors.OrderByDescending(c => c.Contributions).ToList();
    foreach ((string? username, int contributions) in organizationContributors)
    {
        Console.WriteLine($"{username}\t{contributions}");
    }
}