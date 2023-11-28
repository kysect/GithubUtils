using Microsoft.Extensions.Logging;
using Octokit;

namespace Kysect.GithubUtils.Contributions;

public class OrganizationContributionFetcher
{
    private readonly GitHubClient _client;
    private readonly ILogger _logger;

    public OrganizationContributionFetcher(GitHubClient client, ILogger logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<List<OrganizationContributor>> FetchOrganizationStatistic(string organizationName)
    {
        _logger.LogInformation($"Start fetching contributors from {organizationName}");

        IReadOnlyList<Repository> repositories = await _client.Repository.GetAllForOrg(organizationName);

        List<RepositoryContributor> contributors = new List<RepositoryContributor>();
        foreach (Repository repository in repositories)
        {
            _logger.LogDebug($"Try fetch contributions from {repository.FullName}");
            IReadOnlyList<RepositoryContributor> repositoryContributors = await _client.Repository.GetAllContributors(repository.Id);
            contributors.AddRange(repositoryContributors);
        }

        return contributors
            .GroupBy(c => c.Login)
            .Select(g => new OrganizationContributor(g.Key, g.Sum(c => c.Contributions)))
            .ToList();
    }
}

public record OrganizationContributor(string Username, int Contributions);