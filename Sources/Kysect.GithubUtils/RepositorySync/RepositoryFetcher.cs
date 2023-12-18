using Kysect.GithubUtils.OrganizationReplication.PathProvider;
using Kysect.GithubUtils.RepositorySync.IPathFormatStrategies;
using Kysect.GithubUtils.RepositorySync.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace Kysect.GithubUtils.RepositorySync;

public class RepositoryFetcher
{
    private readonly RepositoryFetchOptions _fetchOptions;
    private readonly ILogger _logger;

    public RepositoryFetcher(RepositoryFetchOptions fetchOptions, ILogger logger)
    {
        _fetchOptions = fetchOptions;
        _logger = logger;
    }

    public string EnsureRepositoryUpdated(IPathFormatStrategy pathFormatter, GithubRepository githubRepository)
    {
        try
        {
            return EnsureRepositoryUpdatedInternal();
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }

        string EnsureRepositoryUpdatedInternal()
        {
            string targetPath = pathFormatter.GetPathToRepository(githubRepository);

            if (CloneRepositoryIfNeed(targetPath, githubRepository))
                return targetPath;

            _logger.LogDebug($"Try to fetch updates from remote repository. Repository: {githubRepository}, folder: {targetPath}");

            using var repo = new Repository(targetPath);
            Remote remote = repo.Network.Remotes["origin"];
            List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
            Commands.Fetch(repo, remote.Name, refSpecs, _fetchOptions.FetchOptions, string.Empty);
            return targetPath;
        }
    }

    public string Checkout(IPathFormatStrategy pathFormatter, GithubRepositoryBranch repositoryWithBranch)
    {
        _logger.LogDebug($"Checkout branch: {repositoryWithBranch}");
        string targetPath = pathFormatter.GetPathToRepositoryWithBranch(repositoryWithBranch);
        _logger.LogDebug($"Branch for {repositoryWithBranch}: {targetPath}");
        CloneRepositoryIfNeed(targetPath, repositoryWithBranch.GetRepository());

        try
        {
            return CheckoutInternal();
        }
        catch (Exception e)
        {
            var message = $"Exception while checkout {repositoryWithBranch}";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }

        string CheckoutInternal()
        {
            using var repo = new Repository(targetPath);
            Branch selectedBranch = repo.Branches[repositoryWithBranch.Branch];
            if (selectedBranch is null)
            {
                _logger.LogTrace($"Branch {repositoryWithBranch} was not found, try to use origin/{selectedBranch}");
                selectedBranch = repo.Branches[$"origin/{selectedBranch}"];
            }

            if (selectedBranch is null)
            {
                var message = $"Specified branch was not found: {repositoryWithBranch}";
                _logger.LogError(message);
                _logger.LogInformation("Available branches: " + string.Join(", ", repo.Branches.Select(b => b.FriendlyName)));

                if (!_fetchOptions.IgnoreMissedBranch)
                {
                    _logger.LogError($"Failed to checkout {repositoryWithBranch}. Branch was not found.");
                    throw new ArgumentException(message);
                }

                _logger.LogWarning($"Skip checkout to branch {repositoryWithBranch}. No such branch in repository.");

                return targetPath;
            }

            Remote remote = repo.Network.Remotes["origin"];
            List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
            Commands.Fetch(repo, remote.Name, refSpecs, _fetchOptions.FetchOptions, string.Empty);
            Commands.Checkout(repo, selectedBranch, _fetchOptions.CheckoutOptions);
            return targetPath;
        }
    }

    public void CloneAllBranches(IOrganizationReplicatorPathFormatter pathFormatter, GithubRepository githubRepository)
    {
        string masterClonePath = pathFormatter.GetPathToRepository(githubRepository);

        _logger.LogDebug($"Try to clone all branches from {githubRepository} to {masterClonePath}");
        CloneRepositoryIfNeed(masterClonePath, githubRepository);
        using var gitRepository = new Repository(masterClonePath);

        IReadOnlyCollection<GithubRepositoryBranch> branches = EnumerateBranches(gitRepository, githubRepository);
        _logger.LogDebug($"Discovered {branches.Count} branches for {githubRepository}");

        foreach (GithubRepositoryBranch branch in branches)
        {
            Checkout(pathFormatter, branch);
        }
    }

    private bool CloneRepositoryIfNeed(string targetPath, GithubRepository githubRepository)
    {
        string remoteUrl = githubRepository.ToGithubGitUrl();

        if (Directory.Exists(targetPath))
            return false;

        _logger.LogDebug($"Create directory for cloning repo. Repository: {githubRepository}, folder: {targetPath}");
        Directory.CreateDirectory(targetPath);
        Repository.Clone(remoteUrl, targetPath, _fetchOptions.CloneOptions);
        return true;
    }

    private static IReadOnlyCollection<GithubRepositoryBranch> EnumerateBranches(Repository gitRepository, GithubRepository githubRepository)
    {
        return gitRepository
            .Branches
            .Where(b => b.FriendlyName.StartsWith("origin/"))
            .Select(b => b.FriendlyName)
            .Select(b => b.Substring("remote/".Length))
            .Select(b => new GithubRepositoryBranch(githubRepository, b))
            .ToList();
    }
}