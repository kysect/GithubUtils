using Octokit;
using Serilog;

namespace Kysect.GithubUtils.OrganizationContributions;

public class OrganizationContributionFetcher
{
    private readonly GitHubClient _client;

    public OrganizationContributionFetcher(GitHubClient client)
    {
        _client = client;
    }

    public async Task<List<OrganizationContributor>> FetchOrganizationStatistic(string organizationName)
    {
        Log.Information($"Start fetching contributors from {organizationName}");

        IReadOnlyList<Repository> repositories = await _client.Repository.GetAllForOrg(organizationName);

        List<RepositoryContributor> contributors = new List<RepositoryContributor>();
        foreach (Repository repository in repositories)
        {
            Log.Debug($"Try fetch contributions from {repository.FullName}");
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