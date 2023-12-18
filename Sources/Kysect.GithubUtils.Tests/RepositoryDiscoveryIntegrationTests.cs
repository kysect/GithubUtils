using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.GithubUtils.Replication.OrganizationsSync.RepositoryDiscovering;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Octokit;

namespace Kysect.GithubUtils.Tests;

//to pass tests you should specify some information in appsettings.json
[TestFixture]
[Ignore("Only manual run")]
public class RepositoryDiscoveryIntegrationTests
{
    private IRepositoryDiscoveryService _discoveryService = null!;
    private string _productionToken = null!;
    private string _organisationName = null!;
    private IConfiguration _configuration = null!;

    [SetUp]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false);
        _configuration = builder.Build();
        _productionToken = _configuration.GetSection("Github").GetSection("Token").Value.ThrowIfNull();
        _organisationName = _configuration.GetSection("Github").GetSection("OrganisationName").Value.ThrowIfNull();

        _discoveryService = new GitHubRepositoryDiscoveryService(new GitHubClient(new ProductHeaderValue("clientName"))
        {
            Credentials = new Credentials(_productionToken)
        });
    }

    [Test]
    public void ShouldCreateGitHubRepositoryDiscoveryService_Successful()
    {
        Assert.IsNotNull(_discoveryService);
    }

    [Test]
    public async Task ShouldBeAbleToGetOrganisationRepositories()
    {
        string expectedRepoName = _configuration.GetSection("ExpectedRepoName").Value.ThrowIfNull();

        IReadOnlyList<Repository> repos = await _discoveryService.GetRepositories(_organisationName);
        Assert.NotNull(repos);
        CollectionAssert.IsNotEmpty(repos);
        // Also checking that pagination is working (pageSize = 100)
        Assert.Greater(repos.Count, 100);
        Assert.NotNull(repos.FirstOrDefault(repo => repo.Name == expectedRepoName),
            "Expected template repository was not found");
    }
}