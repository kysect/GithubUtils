using Kysect.GithubUtils.RepositorySync.LocalStoragePathFactories;

namespace Kysect.GithubUtils.OrganizationReplication.PathProvider;

public interface IOrganizationReplicatorPathFormatter : ILocalStoragePathFactory
{
    string GetPathToOrganizations();
    string GetPathToOrganization(string organization);
    string GetPathToOrganizationWithBranch(string organization, string branch);
}