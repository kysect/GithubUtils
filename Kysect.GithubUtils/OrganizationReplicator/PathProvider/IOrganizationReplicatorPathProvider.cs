namespace Kysect.GithubUtils.OrganizationReplicator;

public interface IOrganizationReplicatorPathProvider
{
    string GetPathToOrganizations();
    string GetPathToOrganization(string organization);
    string GetPathToRepository(string organization, string repository);
    string GetPathToRepositoryWithBranch(string organization, string repository, string branch);
}