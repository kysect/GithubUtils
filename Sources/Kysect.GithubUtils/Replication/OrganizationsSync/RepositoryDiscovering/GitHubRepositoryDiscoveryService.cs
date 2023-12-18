using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.GithubUtils.Models;
using Octokit;

namespace Kysect.GithubUtils.Replication.OrganizationsSync.RepositoryDiscovering;

public sealed class GitHubRepositoryDiscoveryService : IRepositoryDiscoveryService
{
    private const int PageSize = 100;

    private readonly IGitHubClient _gitHubClient;

    public GitHubRepositoryDiscoveryService(IGitHubClient gitHubClient)
    {
        _gitHubClient = gitHubClient.ThrowIfNull();
    }

    public async Task<IReadOnlyList<GithubRepository>> GetRepositories(string organization)
    {
        // TODO: support getting for User also
        IReadOnlyList<Repository> repositories = await _gitHubClient.Repository.GetAllForOrg(organization, new ApiOptions { PageSize = PageSize });

        return repositories
            .Select(r => new GithubRepository(r.Owner.Name, r.Name))
            .ToList();
    }
}