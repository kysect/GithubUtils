using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.OrganizationReplication;
using LibGit2Sharp;
using Serilog;

namespace Kysect.GithubUtils.RepositorySync;

public class RepositoryFetcher
{
    private readonly RepositoryFetchOptions _fetchOptions;

    public RepositoryFetcher(RepositoryFetchOptions fetchOptions)
    {
        ArgumentNullException.ThrowIfNull(fetchOptions);

        _fetchOptions = fetchOptions;
    }

    public string EnsureRepositoryUpdated(IPathToRepositoryProvider pathProvider, GithubRepository githubRepository)
    {
        try
        {
            return EnsureRepositoryUpdatedInternal();
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            Log.Error($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }

        string EnsureRepositoryUpdatedInternal()
        {
            string targetPath = pathProvider.GetPathToRepository(githubRepository);

            if (CloneRepositoryIfNeed(targetPath, githubRepository))
                return targetPath;

            Log.Debug($"Try to fetch updates from remote repository. Repository: {githubRepository}, folder: {targetPath}");

            using var repo = new Repository(targetPath);
            Remote remote = repo.Network.Remotes["origin"];
            List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
            Commands.Fetch(repo, remote.Name, refSpecs, _fetchOptions.FetchOptions, string.Empty);
            return targetPath;
        }
    }

    public string Checkout(IPathToRepositoryProvider pathProvider, GithubRepositoryBranch repositoryWithBranch)
    {
        ArgumentNullException.ThrowIfNull(pathProvider);

        Log.Debug($"Checkout branch: {repositoryWithBranch}");
        string targetPath = pathProvider.GetPathToRepositoryWithBranch(repositoryWithBranch);
        Log.Debug($"Branch for {repositoryWithBranch}: {targetPath}");
        CloneRepositoryIfNeed(targetPath, repositoryWithBranch.GetRepository());

        try
        {
            return CheckoutInternal();
        }
        catch (Exception e)
        {
            var message = $"Exception while checkout {repositoryWithBranch}";
            Log.Error($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }

        string CheckoutInternal()
        {
            using var repo = new Repository(targetPath);
            Branch selectedBranch = repo.Branches[repositoryWithBranch.Branch];
            if (selectedBranch is null)
            {
                Log.Verbose($"Branch {repositoryWithBranch} was not found, try to use origin/{selectedBranch}");
                selectedBranch = repo.Branches[$"origin/{selectedBranch}"];
            }

            if (selectedBranch is null)
            {
                var message = $"Specified branch was not found: {repositoryWithBranch}";
                Log.Error(message);
                Log.Information("Available branches: " + string.Join(", ", repo.Branches.Select(b => b.FriendlyName)));

                if (!_fetchOptions.IgnoreMissedBranch)
                {
                    Log.Error($"Failed to checkout {repositoryWithBranch}. Branch was not found.");
                    throw new ArgumentException(message);
                }

                Log.Warning($"Skip checkout to branch {repositoryWithBranch}. No such branch in repository.");

                return targetPath;
            }

            Remote remote = repo.Network.Remotes["origin"];
            List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
            Commands.Fetch(repo, remote.Name, refSpecs, _fetchOptions.FetchOptions, string.Empty);
            Commands.Checkout(repo, selectedBranch, _fetchOptions.CheckoutOptions);
            return targetPath;
        }
    }

    public void CloneAllBranches(IOrganizationReplicatorPathProvider pathProvider, GithubRepository githubRepository)
    {
        string masterClonePath = pathProvider.GetPathToRepository(githubRepository);

        Log.Debug($"Try to clone all branches from {githubRepository} to {masterClonePath}");
        CloneRepositoryIfNeed(masterClonePath, githubRepository);
        using var gitRepository = new Repository(masterClonePath);

        IReadOnlyCollection<GithubRepositoryBranch> branches = EnumerateBranches(gitRepository, githubRepository);
        Log.Debug($"Discovered {branches.Count} branches for {githubRepository}");

        foreach (GithubRepositoryBranch branch in branches)
        {
            Checkout(pathProvider, branch);
        }
    }

    private bool CloneRepositoryIfNeed(string targetPath, GithubRepository githubRepository)
    {
        string remoteUrl = githubRepository.ToGithubGitUrl();

        if (Directory.Exists(targetPath))
            return false;
        
        Log.Debug($"Create directory for cloning repo. Repository: {githubRepository}, folder: {targetPath}");
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