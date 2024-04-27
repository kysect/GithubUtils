using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.Replication.OrganizationsSync.LocalStoragePathFactories;

public interface ILocalStoragePathFactory
{
    string GetPathToRepository(GithubRepository repository);
    string GetPathToRepositoryWithBranch(GithubRepository repository, string branch);
}