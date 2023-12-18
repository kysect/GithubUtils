using Kysect.CommonLib.BaseTypes.Extensions;
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

    public Task<IReadOnlyList<Repository>> GetRepositories(string organization)
    {
        // TODO: support getting for User also
        return _gitHubClient.Repository.GetAllForOrg(organization, new ApiOptions { PageSize = PageSize });
    }
}