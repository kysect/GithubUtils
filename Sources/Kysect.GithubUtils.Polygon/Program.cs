using Kysect.CommonLib.DependencyInjection.Logging;
using Kysect.GithubUtils.Polygon;
using Octokit;

var logger = DefaultLoggerConfiguration.CreateConsoleLogger();

string token = "token";

var client = new GitHubClient(new ProductHeaderValue("Kysect"))
{
    Credentials = new Credentials(token)
};

var demoScenarios = new DemoScenarios(logger);
demoScenarios.CheckFetcher();
