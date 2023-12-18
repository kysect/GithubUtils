using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.Replication.OrganizationsSync.RepositoryDiscovering;

public interface IRepositoryDiscoveryService
{
    Task<IReadOnlyList<GithubRepository>> GetRepositories(string organization);
}