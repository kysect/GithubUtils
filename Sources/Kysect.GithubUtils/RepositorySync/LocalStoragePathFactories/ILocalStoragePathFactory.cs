using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.RepositorySync.LocalStoragePathFactories;

public interface ILocalStoragePathFactory
{
    string GetPathToRepository(GithubRepository repository);
    string GetPathToRepositoryWithBranch(GithubRepositoryBranch repositoryBranch);
}