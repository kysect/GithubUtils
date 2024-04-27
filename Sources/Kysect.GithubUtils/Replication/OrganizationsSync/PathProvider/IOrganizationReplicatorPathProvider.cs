using Kysect.GithubUtils.Replication.OrganizationsSync.LocalStoragePathFactories;

namespace Kysect.GithubUtils.Replication.OrganizationsSync.PathProvider;

public interface IOrganizationReplicatorPathFormatter : ILocalStoragePathFactory
{
    string GetPathToOrganizations();
    string GetPathToOrganization(string organization);
    string GetPathToOrganizationWithBranch(string organization, string branch);
}