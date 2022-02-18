using LibGit2Sharp;
using Serilog;

namespace Kysect.GithubUtils;

public class RepositoryFetcher
{
    private readonly IPathFormatter _pathFormatter;
    private readonly string _gitUser;
    private readonly string _token;
    private readonly RepositoryFetchOptions _fetchOptions;

    public RepositoryFetcher(IPathFormatter pathFormatter, string gitUser, string token, RepositoryFetchOptions fetchOptions)
    {
        ArgumentNullException.ThrowIfNull(pathFormatter);
        ArgumentNullException.ThrowIfNull(gitUser);
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(fetchOptions);

        _pathFormatter = pathFormatter;
        _gitUser = gitUser;
        _token = token;
        _fetchOptions = fetchOptions;
    }

    public string EnsureRepositoryUpdated(string username, string repository)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(repository);

        try
        {
            return EnsureRepositoryUpdatedInternal(username, repository);
        }
        catch (Exception e)
        {
            var message = $"Exception while updating repo: {username}/{repository}";
            throw new GithubUtilsException(message, e);
        }
    }

    private string EnsureRepositoryUpdatedInternal(string username, string repository)
    {
        string remoteUrl = GetRemoteUrl(username, repository);
        string targetPath = _pathFormatter.FormatFolderPath(username, repository);

        if (!Directory.Exists(targetPath))
        {
            Log.Debug($"Create directory for cloning repo. Repository: {username}/{repository}, folder: {targetPath}");
            Directory.CreateDirectory(targetPath);
            var cloneOptions = new CloneOptions { CredentialsProvider = CreateCredentialsProvider };
            Repository.Clone(remoteUrl, targetPath, cloneOptions);
            return targetPath;
        }

        Log.Debug($"Try to fetch updates from remote repository. Repository: {username}/{repository}, folder: {targetPath}");
        using var repo = new Repository(targetPath);
        var fetchOptions = new FetchOptions { CredentialsProvider = CreateCredentialsProvider };
        Remote remote = repo.Network.Remotes["origin"];
        List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
        Commands.Fetch(repo, remote.Name, refSpecs, fetchOptions, string.Empty);
        return targetPath;

    }

    public string Checkout(string username, string repository, string branch)
    {
        Log.Debug($"Checkout branch. Repository: {username}/{repository}, branch: {branch}");
        
        string targetPath = _pathFormatter.FormatFolderPath(username, repository);
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
                var message = $"Specified branch was not found. Repository: {repository}, branch: {branch}";
                Log.Error(message);
                Log.Information("Available branches: " + string.Join(", ", repo.Branches.Select(b => b.FriendlyName)));

                if (!_fetchOptions.IgnoreMissedBranch)
                    throw new ArgumentException(message);
                else
                    Log.Debug($"Skip checkout to branch {branch}. No such branch in {username}/{repository}.");
                
                return targetPath;
            }

            Commands.Checkout(repo, repoBranch, _fetchOptions.CheckoutOptions);
            return targetPath;
        }
        catch (Exception e)
        {
            var message = $"Exception while checkout repo: {username}/{repository}, branch: {branch}";
            throw new GithubUtilsException(message, e);
        }
    }

    private UsernamePasswordCredentials CreateCredentialsProvider(string url, string usernameFromUrl, SupportedCredentialTypes types)
    {
        return new UsernamePasswordCredentials { Username = _gitUser, Password = _token };
    }

    private string GetRemoteUrl(string username, string repository)
    {
        return $"https://github.com/{username}/{repository}.git";
    }
}