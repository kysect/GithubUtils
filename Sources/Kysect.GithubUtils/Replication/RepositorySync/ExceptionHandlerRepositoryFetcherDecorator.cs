using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.GithubUtils.Models;
using Microsoft.Extensions.Logging;

namespace Kysect.GithubUtils.Replication.RepositorySync;

public class ExceptionHandlerRepositoryFetcherDecorator : IRepositoryFetcher
{
    private readonly IRepositoryFetcher _fetcher;
    private readonly ILogger _logger;

    public ExceptionHandlerRepositoryFetcherDecorator(IRepositoryFetcher fetcher, ILogger logger)
    {
        _logger = logger;
        _fetcher = fetcher.ThrowIfNull();
    }

    public bool CloneRepositoryIfNeed(string targetPath, IRemoteGitRepository remoteRepository)
    {
        try
        {
            return _fetcher.CloneRepositoryIfNeed(targetPath, remoteRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {remoteRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public bool Clone(string targetPath, IRemoteGitRepository remoteRepository)
    {
        try
        {
            return _fetcher.Clone(targetPath, remoteRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {remoteRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public void FetchAllBranches(string targetPath, IRemoteGitRepository remoteRepository)
    {
        try
        {
            _fetcher.FetchAllBranches(targetPath, remoteRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {remoteRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public void CheckoutBranch(string targetPath, IRemoteGitRepository remoteRepository, string branch)
    {
        try
        {
            _fetcher.CheckoutBranch(targetPath, remoteRepository, branch);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {remoteRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public string EnsureRepositoryUpdated(string targetPath, IRemoteGitRepository remoteRepository)
    {
        try
        {
            return _fetcher.EnsureRepositoryUpdated(targetPath, remoteRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {remoteRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public string Checkout(string targetPath, IRemoteGitRepository remoteRepository, string branch)
    {
        try
        {
            return _fetcher.Checkout(targetPath, remoteRepository, branch);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {remoteRepository}, Branch: {branch}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public IReadOnlyCollection<string> GetAllRemoteBranches(string targetPath, IRemoteGitRepository remoteRepository)
    {
        try
        {
            return _fetcher.GetAllRemoteBranches(targetPath, remoteRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {remoteRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }
}