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

    public bool CloneRepositoryIfNeed(string targetPath, GithubRepository githubRepository)
    {
        if (IsRepositoryCloned(targetPath))
            return false;

        return Clone(targetPath, githubRepository);
    }

    public bool Clone(string targetPath, GithubRepository githubRepository)
    {
        _logger.LogDebug($"Create directory for cloning repo. Repository: {githubRepository}, folder: {targetPath}");
        Directory.CreateDirectory(targetPath);
        string remoteUrl = githubRepository.ToGithubGitUrl();
        Repository.Clone(remoteUrl, targetPath, _fetchOptions.CloneOptions);
        return true;
    }

    public void FetchAllBranches(string targetPath, GithubRepository githubRepository)
    {
        _logger.LogDebug($"Try to fetch updates from remote repository. Repository: {githubRepository}, folder: {targetPath}");
        using var repo = new Repository(targetPath);

        Remote remote = repo.Network.Remotes["origin"];
        List<string> refSpecs = remote.FetchRefSpecs
            .Select(x => x.Specification)
            .ToList();

        Commands.Fetch(repo, remote.Name, refSpecs, _fetchOptions.FetchOptions, string.Empty);
    }

    public void CheckoutBranch(string targetPath, GithubRepository githubRepository, string branch)
    {
        using var repo = new Repository(targetPath);
        Branch selectedBranch = repo.Branches[branch];
        if (selectedBranch is null)
        {
            _logger.LogTrace($"{githubRepository}, Branch: {branch} was not found, try to use origin/{selectedBranch}");
            selectedBranch = repo.Branches[$"origin/{selectedBranch}"];
        }

        if (selectedBranch is null)
        {
            var message = $"Specified branch was not found ({githubRepository}, Branch: {branch})";
            _logger.LogError(message);
            _logger.LogInformation("Available branches: " + string.Join(", ", repo.Branches.Select(b => b.FriendlyName)));

            if (!_fetchOptions.IgnoreMissedBranch)
            {
                _logger.LogError($"Failed to checkout {githubRepository}, Branch: {branch}. Branch was not found.");
                throw new ArgumentException(message);
            }

            _logger.LogWarning($"Skip checkout to branch {githubRepository}, Branch: {branch}. No such branch in repository.");
            return;
        }

        Commands.Checkout(repo, selectedBranch, _fetchOptions.CheckoutOptions);
    }

    public string EnsureRepositoryUpdated(string targetPath, GithubRepository githubRepository)
    {
        if (CloneRepositoryIfNeed(targetPath, githubRepository))
            return targetPath;

        FetchAllBranches(targetPath, githubRepository);
        return targetPath;
    }

    public string Checkout(string targetPath, GithubRepository repository, string branch)
    {
        _logger.LogDebug($"Checkout branch: {repository}, Branch: {branch}");

        CloneRepositoryIfNeed(targetPath, repository);
        FetchAllBranches(targetPath, repository);
        CheckoutBranch(targetPath, repository, branch);
        return targetPath;
    }

    public IReadOnlyCollection<string> GetAllRemoteBranches(string targetPath, GithubRepository githubRepository)
    {
        using var gitRepository = new Repository(targetPath);
        CloneRepositoryIfNeed(targetPath, githubRepository);

        // TODO: looks like a bug, need to check
        IReadOnlyCollection<string> branches = gitRepository
            .Branches
            .Where(b => b.FriendlyName.StartsWith("origin/"))
            .Select(b => b.FriendlyName)
            .Select(b => b.Substring("remote/".Length))
            .ToList();

        _logger.LogDebug($"Discovered {branches.Count} branches for {githubRepository}");
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