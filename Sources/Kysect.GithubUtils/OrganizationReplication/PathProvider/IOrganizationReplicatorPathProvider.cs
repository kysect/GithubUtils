using Kysect.GithubUtils.RepositorySync.IPathFormatStrategies;

namespace Kysect.GithubUtils.OrganizationReplication.PathProvider;

public interface IOrganizationReplicatorPathFormatter : IPathFormatStrategy
{
    string GetPathToOrganizations();
    string GetPathToOrganization(string organization);
    string GetPathToOrganizationWithBranch(string organization, string branch);
}