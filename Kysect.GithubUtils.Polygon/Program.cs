using Kysect.GithubUtils;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var gitUser = string.Empty;
var token = string.Empty;
var repositoryFetcher = new RepositoryFetcher(new FullPathFormatter("repo"), gitUser, token);
repositoryFetcher.EnsureRepositoryUpdated("fredikats", "test");
repositoryFetcher.Checkout("fredikats", "test", "main");
repositoryFetcher.Checkout("fredikats", "test", "qq");