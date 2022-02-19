using System.Text.Json;
using Kysect.GithubUtils;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.RepositorySync;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

CheckFetcher();
CheckStatParser();

void CheckFetcher()
{
    var gitUser = string.Empty;
    var token = string.Empty;
    var repositoryFetcher = new RepositoryFetcher(gitUser, token, new RepositoryFetchOptions());
    var githubRepository = new GithubRepository("fredikats", "test");
    repositoryFetcher.EnsureRepositoryUpdated(new FullPathFormatter("repo"), githubRepository);
    repositoryFetcher.Checkout(new FullPathFormatter("repo"), githubRepository, "main");
    repositoryFetcher.Checkout(new FullPathFormatter("repo"), githubRepository, "qq");
}

void CheckStatParser()
{
    IGithubActivityProvider provider = new GithubActivityProvider();
    ActivityInfo activityInfo = provider.GetActivityInfo("FrediKats");
    Console.WriteLine(JsonSerializer.Serialize(activityInfo.PerMonthActivity()));
}