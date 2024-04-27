using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.Replication.RepositorySync;

public interface IRepositoryFetcher
{
    bool CloneRepositoryIfNeed(string targetPath, GithubRepository githubRepository);
    bool Clone(string targetPath, GithubRepository githubRepository);
    void FetchAllBranches(string targetPath, GithubRepository githubRepository);
    void CheckoutBranch(string targetPath, GithubRepository githubRepository, string branch);
    string EnsureRepositoryUpdated(string targetPath, GithubRepository githubRepository);
    string Checkout(string targetPath, GithubRepository repository, string branch);
    IReadOnlyCollection<string> GetAllRemoteBranches(string targetPath, GithubRepository githubRepository);
}