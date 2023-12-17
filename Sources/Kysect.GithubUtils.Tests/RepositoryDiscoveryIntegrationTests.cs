using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kysect.GithubUtils.RepositoryDiscovering;
using Microsoft.Extensions.Configuration;

namespace Kysect.GithubUtils.Tests;

//to pass tests you should specify some information in appsettings.json

[Ignore("Only manual run")]
public class RepositoryDiscoveryIntegrationTests
{
    private IRepositoryDiscoveryService _discoveryService;
    private string _productionToken;
    private string _organisationName;
    private IConfiguration _configuration;

    [SetUp] 
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(
                Directory.GetParent(Directory.GetParent(Directory.GetParent(
                        Directory.GetCurrentDirectory())!
                    .ToString())?.ToString() ?? string.Empty)?.ToString())
            .AddJsonFile("appsettings.json", optional: false);
        _configuration = builder.Build();
        _productionToken = _configuration.GetSection("Github").GetSection("Token").Value;
        _organisationName = _configuration.GetSection("Github").GetSection("OrganisationName").Value;
    }
    
    [Test]
    public void ShouldCreateGitHubRepositoryDiscoveryService_Successful()
    {
        _discoveryService = new GitHubRepositoryDiscoveryService(_productionToken);
        Assert.IsNotNull(_discoveryService);
    }

    [Test]
    public void ShouldNotCreateWithoutToken_ThrowsConfigurationException()
    {
        var exception = Assert.Catch<RepositoryDiscoveryConfigurationException>(() =>
        {
            _discoveryService = new GitHubRepositoryDiscoveryService(string.Empty);
        });

        Assert.NotNull(exception.InnerException);
        Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
    }

    [Test]
    public void ShouldNotDiscoverWithoutOrganization_ThrowsConfigurationException()
    {
        _discoveryService = new GitHubRepositoryDiscoveryService(_productionToken);
        var exception = Assert.ThrowsAsync<RepositoryDiscoveryConfigurationException>(async () =>
        {
            await _discoveryService.TryDiscover(string.Empty).ToListAsync();
        });

        Assert.NotNull(exception.InnerException);
        Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
    }

    [TestCase("ghp-not-valid-token")]
    public void ShouldHandleUnauthorizedScenarios_ThrowsRuntimeException(string token)
    {
        _discoveryService = new GitHubRepositoryDiscoveryService(token);
        List<RepositoryRecord> repos = null;
        var exception = Assert.ThrowsAsync<RepositoryDiscoveryGenericException>(async () =>
        {
            repos = await _discoveryService.TryDiscover(_organisationName).ToListAsync();
        });

        Assert.Null(repos);
        Assert.NotNull(exception);
        Assert.IsTrue(exception.Message.Contains("Invalid token"), "Not excepted exception was handled");
    }

    [TestCase("that-organization-should-not-exist")]
    public void ShouldHandleOrganizationNotFoundScenario_ThrowsRuntimeException(string org)
    {
        _discoveryService = new GitHubRepositoryDiscoveryService(_productionToken);
        List<RepositoryRecord> repos = null;
        var exception = Assert.ThrowsAsync<RepositoryDiscoveryGenericException>(async () =>
        {
            repos = await _discoveryService.TryDiscover(org).ToListAsync();
        });

        Assert.Null(repos);
        Assert.NotNull(exception);
        Assert.IsTrue(exception.Message.Contains("organization was not found"),
            "Not excepted exception was handled");
    }

    public async Task ShouldBeAbleToGetOrganisationRepositories()
    {
        string expectedRepoName = _configuration.GetSection("ExpectedRepoName").Value;
        _discoveryService = new GitHubRepositoryDiscoveryService(_productionToken);
        var repos = await _discoveryService.TryDiscover(_organisationName).ToListAsync();
        Assert.NotNull(repos);
        CollectionAssert.IsNotEmpty(repos);
        // Also checking that pagination is working (pageSize = 100)
        Assert.Greater(repos.Count(), 100);
        Assert.NotNull(repos.FirstOrDefault(repo => repo.Name == expectedRepoName),
            "Expected template repository was not found");
    }
}