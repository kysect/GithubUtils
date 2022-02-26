using Kysect.GithubUtils.RepositorySync;

namespace Kysect.GithubUtils.OrganizationReplication;

public interface IOrganizationReplicatorPathFormatter : IPathFormatStrategy
{
    string GetPathToOrganizations();
    string GetPathToOrganization(string organization);
    string GetPathToOrganizationWithBranch(string organization, string branch);
}