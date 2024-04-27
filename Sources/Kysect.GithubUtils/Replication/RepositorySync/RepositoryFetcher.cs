using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.Replication.RepositorySync.LocalStoragePathFactories;
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

    public string EnsureRepositoryUpdated(ILocalStoragePathFactory pathFormatter, GithubRepository githubRepository)
    {
        pathFormatter.ThrowIfNull();

        string targetPath = pathFormatter.GetPathToRepository(githubRepository);

        if (CloneRepositoryIfNeed(targetPath, githubRepository))
            return targetPath;

        _logger.LogDebug($"Try to fetch updates from remote repository. Repository: {githubRepository}, folder: {targetPath}");

        using var repo = new Repository(targetPath);
        Remote remote = repo.Network.Remotes["origin"];
        List<string> refSpecs = remote.FetchRefSpecs
            .Select(x => x.Specification)
            .ToList();

        Commands.Fetch(repo, remote.Name, refSpecs, _fetchOptions.FetchOptions, string.Empty);
        return targetPath;
    }

    public string Checkout(ILocalStoragePathFactory pathFormatter, GithubRepository repository, string branch, bool directoryPerBranch = false)
    {
        pathFormatter.ThrowIfNull();
        _logger.LogDebug($"Checkout branch: {repository}, Branch: {branch}");

        string targetPath = directoryPerBranch
            ? pathFormatter.GetPathToRepositoryWithBranch(repository, branch)
            : pathFormatter.GetPathToRepository(repository);

        CloneRepositoryIfNeed(targetPath, repository);

        using var repo = new Repository(targetPath);
        Branch selectedBranch = repo.Branches[branch];
        if (selectedBranch is null)
        {
            _logger.LogTrace($"{repository}, Branch: {branch} was not found, try to use origin/{selectedBranch}");
            selectedBranch = repo.Branches[$"origin/{selectedBranch}"];
        }

        if (selectedBranch is null)
        {
            var message = $"Specified branch was not found ({repository}, Branch: {branch})";
            _logger.LogError(message);
            _logger.LogInformation("Available branches: " + string.Join(", ", repo.Branches.Select(b => b.FriendlyName)));

            if (!_fetchOptions.IgnoreMissedBranch)
            {
                _logger.LogError($"Failed to checkout {repository}, Branch: {branch}. Branch was not found.");
                throw new ArgumentException(message);
            }

            _logger.LogWarning($"Skip checkout to branch {repository}, Branch: {branch}. No such branch in repository.");

            return targetPath;
        }

        Remote remote = repo.Network.Remotes["origin"];
        var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
        Commands.Fetch(repo, remote.Name, refSpecs, _fetchOptions.FetchOptions, string.Empty);
        Commands.Checkout(repo, selectedBranch, _fetchOptions.CheckoutOptions);
        return targetPath;
    }

    public void CloneAllBranches(ILocalStoragePathFactory pathFormatter, GithubRepository githubRepository)
    {
        pathFormatter.ThrowIfNull();

        string masterClonePath = pathFormatter.GetPathToRepository(githubRepository);

        _logger.LogDebug($"Try to clone all branches from {githubRepository} to {masterClonePath}");
        CloneRepositoryIfNeed(masterClonePath, githubRepository);
        using var gitRepository = new Repository(masterClonePath);

        IReadOnlyCollection<string> branches = gitRepository
            .Branches
            .Where(b => b.FriendlyName.StartsWith("origin/"))
            .Select(b => b.FriendlyName)
            .Select(b => b.Substring("remote/".Length))
            .ToList();

        _logger.LogDebug($"Discovered {branches.Count} branches for {githubRepository}");

        foreach (string branch in branches)
        {
            Checkout(pathFormatter, githubRepository, branch, directoryPerBranch: true);
        }
    }

    private bool CloneRepositoryIfNeed(string targetPath, GithubRepository githubRepository)
    {
        // TODO: handle case when directory exists but is not initialized
        if (Directory.Exists(targetPath))
            return false;

        _logger.LogDebug($"Create directory for cloning repo. Repository: {githubRepository}, folder: {targetPath}");
        Directory.CreateDirectory(targetPath);
        string remoteUrl = githubRepository.ToGithubGitUrl();
        Repository.Clone(remoteUrl, targetPath, _fetchOptions.CloneOptions);
        return true;
    }
}