namespace Kysect.GithubUtils.RepositoryDiscovering;

public interface IRepositoryDiscoveryService
{
    IAsyncEnumerable<RepositoryRecord> TryDiscover(string organization, CancellationToken cancellationToken = default);
}