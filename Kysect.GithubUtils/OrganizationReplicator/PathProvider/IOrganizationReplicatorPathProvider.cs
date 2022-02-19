using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.RepositorySync;

namespace Kysect.GithubUtils.OrganizationReplicator;

public interface IOrganizationReplicatorPathProvider : IPathToRepositoryProvider
{
    string GetPathToOrganizations();
    string GetPathToOrganization(string organization);
    string GetPathToOrganizationWithBranch(string organization, string branch);
}