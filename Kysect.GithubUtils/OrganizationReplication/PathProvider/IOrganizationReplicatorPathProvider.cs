using Kysect.GithubUtils.RepositorySync;

namespace Kysect.GithubUtils.OrganizationReplication;

public interface IOrganizationReplicatorPathProvider : IPathToRepositoryProvider
{
    string GetPathToOrganizations();
    string GetPathToOrganization(string organization);
    string GetPathToOrganizationWithBranch(string organization, string branch);
}