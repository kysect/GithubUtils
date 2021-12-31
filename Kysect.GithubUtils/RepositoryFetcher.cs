﻿using LibGit2Sharp;
using Serilog;

namespace Kysect.GithubUtils;

public interface IPathFormatter
{
    string FormatFolderPath(string username, string repository);
}

public class RepositoryFetcher
{
    private readonly IPathFormatter _pathFormatter;
    private readonly string _gitUser;
    private readonly string _token;

    public RepositoryFetcher(IPathFormatter pathFormatter, string gitUser, string token)
    {
        ArgumentNullException.ThrowIfNull(pathFormatter);
        ArgumentNullException.ThrowIfNull(gitUser);
        ArgumentNullException.ThrowIfNull(token);

        _pathFormatter = pathFormatter;
        _gitUser = gitUser;
        _token = token;
    }

    public void EnsureRepositoryUpdated(string username, string repository)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(repository);

        string remoteUrl = GetRemoteUrl(username, repository);
        string targetPath = _pathFormatter.FormatFolderPath(username, repository);

        if (!Directory.Exists(targetPath))
        {
            Log.Information($"Create directory for cloning repo. Repository: {username}/{repository}, folder: {targetPath}");
            Directory.CreateDirectory(targetPath);
            var cloneOptions = new CloneOptions { CredentialsProvider = CreateCredentialsProvider };
            Repository.Clone(remoteUrl, targetPath, cloneOptions);
            return;
        }

        Log.Information($"Try to fetch updates from remote repository. Repository: {username}/{repository}, folder: {targetPath}");
        using var repo = new Repository(targetPath);
        var fetchOptions = new FetchOptions { CredentialsProvider = CreateCredentialsProvider };
        Remote remote = repo.Network.Remotes["origin"];
        List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
        Log.Debug("Refs for update: " + string.Join(", ", refSpecs));
        Commands.Fetch(repo, remote.Name, refSpecs, fetchOptions, string.Empty);
    }

    public void Checkout(string username, string repository, string branch)
    {
        Log.Information($"Checkout branch. Repository: {username}/{repository}, branch: {branch}");
        EnsureRepositoryUpdated(username, repository);
        
        string targetPath = _pathFormatter.FormatFolderPath(username, repository);
        using var repo = new Repository(targetPath);
        Branch repoBranch = repo.Branches[branch];
        if (repoBranch is null)
        {
            Log.Information("Branch was not found will try with prefix origin");
            repoBranch = repo.Branches[$"origin/{branch}"];
        }

        if (repoBranch is null)
            throw new ArgumentException($"Specified branch was not found: {repoBranch}");

        Commands.Checkout(repo, repoBranch);
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