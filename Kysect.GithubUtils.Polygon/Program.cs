using System.Text.Json;
using Kysect.GithubUtils;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

//CheckFetcher();
CheckStatParser();

void CheckFetcher()
{
    var gitUser = string.Empty;
    var token = string.Empty;
    var repositoryFetcher = new RepositoryFetcher(new FullPathFormatter("repo"), gitUser, token, new RepositoryFetchOptions());
    repositoryFetcher.EnsureRepositoryUpdated("fredikats", "test");
    repositoryFetcher.Checkout("fredikats", "test", "main");
    repositoryFetcher.Checkout("fredikats", "test", "qq");
}

void CheckStatParser()
{
    IGithubActivityProvider provider = new GithubActivityProvider();
    ActivityInfo activityInfo = provider.GetActivityInfo("FrediKats");
    Console.WriteLine(JsonSerializer.Serialize(activityInfo.PerMonthActivity()));
}