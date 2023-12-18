using Kysect.CommonLib.DependencyInjection;
using Kysect.GithubUtils.Polygon;
using Octokit;

var logger = PredefinedLogger.CreateConsoleLogger();

string token = "token";

var client = new GitHubClient(new ProductHeaderValue("Kysect"))
{
    Credentials = new Credentials(token)
};

var demoScenarios = new DemoScenarios(logger);
demoScenarios.CheckFetcher();
