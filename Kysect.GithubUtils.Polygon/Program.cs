using System.Text.Json;
using Kysect.GithubUtils;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.OrganizationContributions;
using Kysect.GithubUtils.OrganizationReplication;
using Kysect.GithubUtils.RepositorySync;
using Octokit;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

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
    var repositoryFetcher = new RepositoryFetcher(new RepositoryFetchOptions(gitUser, token));
    var githubRepository = new GithubRepository("fredikats", "test");
    repositoryFetcher.EnsureRepositoryUpdated(new UseOwnerAndRepoForFolderNameStrategy("repo"), githubRepository);
    repositoryFetcher.Checkout(new UseOwnerAndRepoForFolderNameStrategy("repo"), new GithubRepositoryBranch(githubRepository, "main"));
    repositoryFetcher.Checkout(new UseOwnerAndRepoForFolderNameStrategy("repo"), new GithubRepositoryBranch(githubRepository, "qq"));
}

void CheckStatParser()
{
    IGithubActivityProvider provider = new GithubActivityProvider();
    ActivityInfo activityInfo = provider.GetActivityInfo("FrediKats");
    Console.WriteLine(JsonSerializer.Serialize(activityInfo.PerMonthActivity()));
}

void CloneCustomBranch()
{
    var gitUser = "fredikats";
    var token = string.Empty;
    var repositoryFetcher = new RepositoryFetcher(new RepositoryFetchOptions(gitUser, token));
    var organizationReplicatorPathProvider = new OrganizationReplicatorPathFormatter("test-repos");
    var organizationReplicationHub = new OrganizationReplicationHub(organizationReplicatorPathProvider, repositoryFetcher);
    organizationReplicationHub.TryAddOrganization("fredikats");
    OrganizationReplicator organizationReplicator = organizationReplicationHub.GetOrganizationReplicator("fredikats");
    organizationReplicator.Clone("fredikats");
    organizationReplicator.CloneBranch("fredikats", "master");
}

void CheckOrgContributions(GitHubClient client, string organization)
{
    var organizationContributionFetcher = new OrganizationContributionFetcher(client);
    List<OrganizationContributor> organizationContributors = organizationContributionFetcher.FetchOrganizationStatistic(organization).Result;
    organizationContributors = organizationContributors.OrderByDescending(c => c.Contributions).ToList();
    foreach ((string? username, int contributions) in organizationContributors)
    {
        Console.WriteLine($"{username}\t{contributions}");
    }
}