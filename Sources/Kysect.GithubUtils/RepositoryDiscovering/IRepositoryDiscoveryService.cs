using Kysect.GithubUtils.RepositoryDiscovering.Models;

namespace Kysect.GithubUtils.RepositoryDiscovering;

public interface IRepositoryDiscoveryService
{
    IAsyncEnumerable<RepositoryRecord> TryDiscover(
        string organization,
        GitHubRepositoryType repositoryTypeFilter = GitHubRepositoryType.All,
        CancellationToken cancellationToken = default);
}