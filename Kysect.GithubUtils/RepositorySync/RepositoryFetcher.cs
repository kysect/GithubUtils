using Kysect.GithubUtils.Models;
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

    public string EnsureRepositoryUpdated(IPathToRepositoryFormatter pathFormatter, GithubRepository githubRepository)
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
            string remoteUrl = githubRepository.ToGithubGitUrl();
            string targetPath = pathFormatter.FormatFolderPath(githubRepository);

            if (!Directory.Exists(targetPath))
            {
                Log.Debug($"Create directory for cloning repo. Repository: {githubRepository}, folder: {targetPath}");
                Directory.CreateDirectory(targetPath);
                var cloneOptions = new CloneOptions { CredentialsProvider = CreateCredentialsProvider };
                Repository.Clone(remoteUrl, targetPath, cloneOptions);
                return targetPath;
            }

            Log.Debug($"Try to fetch updates from remote repository. Repository: {githubRepository}, folder: {targetPath}");
            using var repo = new Repository(targetPath);
            var fetchOptions = new FetchOptions { CredentialsProvider = CreateCredentialsProvider };
            Remote remote = repo.Network.Remotes["origin"];
            List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
            Commands.Fetch(repo, remote.Name, refSpecs, fetchOptions, string.Empty);
            return targetPath;
        }
    }

    public string Checkout(IPathToRepositoryFormatter pathFormatter, GithubRepository githubRepository, string branch)
    {
        Log.Debug($"Checkout branch. Repository: {githubRepository}, branch: {branch}");
        
        string targetPath = pathFormatter.FormatFolderPath(githubRepository);
        try
        {
            using var repo = new Repository(targetPath);
            Branch repoBranch = repo.Branches[branch];
            if (repoBranch is null)
            {
                repoBranch = repo.Branches[$"origin/{branch}"];
            }

            if (repoBranch is null)
            {
                var message = $"Specified branch was not found. Repository: {githubRepository}, branch: {branch}";
                Log.Error(message);
                Log.Information("Available branches: " + string.Join(", ", repo.Branches.Select(b => b.FriendlyName)));

                if (!_fetchOptions.IgnoreMissedBranch)
                    throw new ArgumentException(message);
                else
                    Log.Debug($"Skip checkout to branch {branch}. No such branch in {githubRepository}.");
                
                return targetPath;
            }

            Commands.Checkout(repo, repoBranch, _fetchOptions.CheckoutOptions);
            return targetPath;
        }
        catch (Exception e)
        {
            var message = $"Exception while checkout repo: {githubRepository}, branch: {branch}";
            throw new GithubUtilsException(message, e);
        }
    }

    private UsernamePasswordCredentials CreateCredentialsProvider(string url, string usernameFromUrl, SupportedCredentialTypes types)
    {
        return new UsernamePasswordCredentials { Username = _gitUser, Password = _token };
    }
}