using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.GithubUtils.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace Kysect.GithubUtils.Replication.RepositorySync;

public class RepositoryFetcher : IRepositoryFetcher
{
    private readonly RepositoryFetchOptions _fetchOptions;
    private readonly ILogger _logger;

    public RepositoryFetcher(RepositoryFetchOptions fetchOptions, ILogger logger)
    {
        _fetchOptions = fetchOptions;
        _logger = logger;
    }

    public bool CloneRepositoryIfNeed(string targetPath, IRemoteGitRepository remoteRepository)
    {
        if (IsRepositoryCloned(targetPath))
            return false;

        return Clone(targetPath, remoteRepository);
    }

    public bool Clone(string targetPath, IRemoteGitRepository remoteRepository)
    {
        remoteRepository.ThrowIfNull();

        _logger.LogDebug($"Create directory for cloning repo. Repository: {remoteRepository}, folder: {targetPath}");
        Directory.CreateDirectory(targetPath);
        string remoteUrl = remoteRepository.GetGitHttpsUrl();
        Repository.Clone(remoteUrl, targetPath, _fetchOptions.CloneOptions);
        return true;
    }

    public void FetchAllBranches(string targetPath, IRemoteGitRepository remoteRepository)
    {
        _logger.LogDebug($"Try to fetch updates from remote repository. Repository: {remoteRepository}, folder: {targetPath}");
        using var repo = new Repository(targetPath);

        Remote remote = repo.Network.Remotes["origin"];
        List<string> refSpecs = remote.FetchRefSpecs
            .Select(x => x.Specification)
            .ToList();

        Commands.Fetch(repo, remote.Name, refSpecs, _fetchOptions.FetchOptions, string.Empty);
    }

    public void CheckoutBranch(string targetPath, IRemoteGitRepository remoteRepository, string branch)
    {
        using var repo = new Repository(targetPath);
        Branch selectedBranch = repo.Branches[branch];
        if (selectedBranch is null)
        {
            _logger.LogTrace($"{remoteRepository}, Branch: {branch} was not found, try to use origin/{selectedBranch}");
            selectedBranch = repo.Branches[$"origin/{selectedBranch}"];
        }

        if (selectedBranch is null)
        {
            var message = $"Specified branch was not found ({remoteRepository}, Branch: {branch})";
            _logger.LogError(message);
            _logger.LogInformation("Available branches: " + string.Join(", ", repo.Branches.Select(b => b.FriendlyName)));

            if (!_fetchOptions.IgnoreMissedBranch)
            {
                _logger.LogError($"Failed to checkout {remoteRepository}, Branch: {branch}. Branch was not found.");
                throw new ArgumentException(message);
            }

            _logger.LogWarning($"Skip checkout to branch {remoteRepository}, Branch: {branch}. No such branch in repository.");
            return;
        }

        Commands.Checkout(repo, selectedBranch, _fetchOptions.CheckoutOptions);
    }

    public string EnsureRepositoryUpdated(string targetPath, IRemoteGitRepository remoteRepository)
    {
        if (CloneRepositoryIfNeed(targetPath, remoteRepository))
            return targetPath;

        FetchAllBranches(targetPath, remoteRepository);
        return targetPath;
    }

    public string Checkout(string targetPath, IRemoteGitRepository remoteRepository, string branch)
    {
        _logger.LogDebug($"Checkout branch: {remoteRepository}, Branch: {branch}");

        CloneRepositoryIfNeed(targetPath, remoteRepository);
        FetchAllBranches(targetPath, remoteRepository);
        CheckoutBranch(targetPath, remoteRepository, branch);
        return targetPath;
    }

    public IReadOnlyCollection<string> GetAllRemoteBranches(string targetPath, IRemoteGitRepository remoteRepository)
    {
        using var gitRepository = new Repository(targetPath);
        CloneRepositoryIfNeed(targetPath, remoteRepository);

        // TODO: looks like a bug, need to check
        IReadOnlyCollection<string> branches = gitRepository
            .Branches
            .Where(b => b.FriendlyName.StartsWith("origin/"))
            .Select(b => b.FriendlyName)
            .Select(b => b.Substring("remote/".Length))
            .ToList();

        _logger.LogDebug($"Discovered {branches.Count} branches for {remoteRepository}");
        return branches;
    }

    private bool IsRepositoryCloned(string targetPath)
    {
        // TODO: handle case when directory exists but is not initialized
        if (Directory.Exists(targetPath))
            return false;

        return true;
    }
}