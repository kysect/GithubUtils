using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.Replication.RepositorySync;

public interface IRepositoryFetcher
{
    bool CloneRepositoryIfNeed(string targetPath, IRemoteGitRepository remoteRepository);
    bool Clone(string targetPath, IRemoteGitRepository remoteRepository);
    void FetchAllBranches(string targetPath, IRemoteGitRepository remoteRepository);
    void CheckoutBranch(string targetPath, IRemoteGitRepository remoteRepository, string branch);
    string EnsureRepositoryUpdated(string targetPath, IRemoteGitRepository remoteRepository);
    string Checkout(string targetPath, IRemoteGitRepository remoteRepository, string branch);
    IReadOnlyCollection<string> GetAllRemoteBranches(string targetPath, IRemoteGitRepository remoteRepository);
}