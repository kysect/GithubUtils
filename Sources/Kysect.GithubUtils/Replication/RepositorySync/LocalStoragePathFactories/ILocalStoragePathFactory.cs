using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.Replication.RepositorySync.LocalStoragePathFactories;

public interface ILocalStoragePathFactory
{
    string GetPathToRepository(GithubRepository repository);
    string GetPathToRepositoryWithBranch(GithubRepositoryBranch repositoryBranch);
}