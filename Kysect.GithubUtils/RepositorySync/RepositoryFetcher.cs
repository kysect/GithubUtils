using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.OrganizationReplication;
using LibGit2Sharp;
using Serilog;

namespace Kysect.GithubUtils.RepositorySync;

public class RepositoryFetcher
{
    private readonly string _gitUser;
    private readonly string _token;
    private readonly RepositoryFetchOptions _fetchOptions;

    public RepositoryFetcher(string gitUser, string token, RepositoryFetchOptions fetchOptions)
    {
        ArgumentNullException.ThrowIfNull(gitUser);
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(fetchOptions);

        _gitUser = gitUser;
        _token = token;
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
            var message = $"Exception while updating repo: {githubRepository}";
            throw new GithubUtilsException(message, e);
        }

        string EnsureRepositoryUpdatedInternal()
        {
            string targetPath = pathProvider.GetPathToRepository(githubRepository);

            if (CloneRepositoryIfNeed(targetPath, githubRepository))
                return targetPath;

            Log.Debug($"Try to fetch updates from remote repository. Repository: {githubRepository}, folder: {targetPath}");
            using var repo = new Repository(targetPath);
            var fetchOptions = new FetchOptions { CredentialsProvider = CreateCredentialsProvider };
            Remote remote = repo.Network.Remotes["origin"];
            List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
            Commands.Fetch(repo, remote.Name, refSpecs, fetchOptions, string.Empty);
            return targetPath;
        }
    }

    public string Checkout(IPathToRepositoryProvider pathProvider, GithubRepositoryBranch repositoryBranch)
    {
        ArgumentNullException.ThrowIfNull(pathProvider);

        Log.Debug($"Checkout branch: {repositoryBranch}");
        string targetPath = pathProvider.GetPathToRepositoryWithBranch(repositoryBranch);
        Log.Debug($"Branch for {repositoryBranch}: {targetPath}");
        CloneRepositoryIfNeed(targetPath, repositoryBranch.GetRepository());

        try
        {
            return CheckoutInternal();
        }
        catch (Exception e)
        {
            var message = $"Exception while checkout branch {repositoryBranch}";
            throw new GithubUtilsException(message, e);
        }

        string CheckoutInternal()
        {
            using var repo = new Repository(targetPath);
            Branch repoBranch = repo.Branches[repositoryBranch.Branch];
            if (repoBranch is null)
            {
                repoBranch = repo.Branches[$"origin/{repositoryBranch}"];
            }

            if (repoBranch is null)
            {
                var message = $"Specified branch was not found: {repositoryBranch}";
                Log.Error(message);
                Log.Information("Available branches: " + string.Join(", ", repo.Branches.Select(b => b.FriendlyName)));

                if (!_fetchOptions.IgnoreMissedBranch)
                    throw new ArgumentException(message);
                else
                    Log.Debug($"Skip checkout to branch {repositoryBranch}. No such branch in repository.");

                return targetPath;
            }

            var fetchOptions = new FetchOptions { CredentialsProvider = CreateCredentialsProvider };
            Remote remote = repo.Network.Remotes["origin"];
            List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
            Commands.Fetch(repo, remote.Name, refSpecs, fetchOptions, string.Empty);
            Commands.Checkout(repo, repoBranch, _fetchOptions.CheckoutOptions);
            return targetPath;
        }
    }

    public void CloneAllBranches(IOrganizationReplicatorPathProvider pathProvider, GithubRepository githubRepository)
    {
        string masterClonePath = pathProvider.GetPathToRepository(githubRepository);
        CloneRepositoryIfNeed(masterClonePath, githubRepository);
        using var gitRepository = new Repository(masterClonePath);

        IReadOnlyCollection<GithubRepositoryBranch> branches = EnumerateBranches(gitRepository, githubRepository);
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
        var cloneOptions = new CloneOptions { CredentialsProvider = CreateCredentialsProvider };
        Repository.Clone(remoteUrl, targetPath, cloneOptions);
        return true;
    }

    private UsernamePasswordCredentials CreateCredentialsProvider(string url, string usernameFromUrl, SupportedCredentialTypes types)
    {
        return new UsernamePasswordCredentials { Username = _gitUser, Password = _token };
    }

    private IReadOnlyCollection<GithubRepositoryBranch> EnumerateBranches(Repository gitRepository, GithubRepository githubRepository)
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