namespace Kysect.GithubUtils.Replication.OrganizationsSync.RepositoryDiscovering;

public interface IRepositoryDiscoveryService
{
    Task<IReadOnlyList<Octokit.Repository>> GetRepositories(string organization);
}