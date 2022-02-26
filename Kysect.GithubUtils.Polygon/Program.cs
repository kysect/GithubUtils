using System.Text.Json;
using Kysect.GithubUtils;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.OrganizationReplication;
using Kysect.GithubUtils.RepositorySync;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

void CheckFetcher()
{
    var gitUser = string.Empty;
    var token = string.Empty;
    var repositoryFetcher = new RepositoryFetcher(new RepositoryFetchOptions(gitUser, token));
    var githubRepository = new GithubRepository("fredikats", "test");
    repositoryFetcher.EnsureRepositoryUpdated(new FullPathProvider("repo"), githubRepository);
    repositoryFetcher.Checkout(new FullPathProvider("repo"), new GithubRepositoryBranch(githubRepository, "main"));
    repositoryFetcher.Checkout(new FullPathProvider("repo"), new GithubRepositoryBranch(githubRepository, "qq"));
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
    var organizationReplicatorPathProvider = new OrganizationReplicatorPathProvider("test-repos");
    var organizationReplicationHub = new OrganizationReplicationHub(organizationReplicatorPathProvider, repositoryFetcher);
    organizationReplicationHub.TryAddOrganization("fredikats");
    OrganizationReplicator organizationReplicator = organizationReplicationHub.GetOrganizationReplicator("fredikats");
    organizationReplicator.Clone("fredikats");
    organizationReplicator.CloneBranch("fredikats", "master");
}
