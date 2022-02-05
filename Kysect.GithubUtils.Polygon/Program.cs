using Kysect.GithubUtils;
using Microsoft.Extensions.Configuration;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var gitUser = string.Empty;
var token = string.Empty;
var builder = new ConfigurationBuilder()
    .SetBasePath(
        Directory.GetParent(Directory.GetParent(Directory.GetParent(
                        Directory.GetCurrentDirectory())!
                    .ToString())?.ToString() ?? string.Empty)?.ToString())
            .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = builder.Build();
var repositoryFetcher = new RepositoryFetcher(new FullPathFormatter("repo"), config);
repositoryFetcher.EnsureRepositoryUpdated("fredikats", "test");
repositoryFetcher.Checkout("fredikats", "test", "main");
repositoryFetcher.Checkout("fredikats", "test", "qq");