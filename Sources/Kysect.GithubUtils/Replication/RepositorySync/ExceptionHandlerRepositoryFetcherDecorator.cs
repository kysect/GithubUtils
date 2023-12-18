using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.Replication.RepositorySync.LocalStoragePathFactories;
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

    public string EnsureRepositoryUpdated(ILocalStoragePathFactory pathFormatter, GithubRepository githubRepository)
    {
        try
        {
            return _fetcher.EnsureRepositoryUpdated(pathFormatter, githubRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public string Checkout(ILocalStoragePathFactory pathFormatter, GithubRepositoryBranch repositoryWithBranch, bool directoryPerBranch = false)
    {
        try
        {
            return _fetcher.Checkout(pathFormatter, repositoryWithBranch, directoryPerBranch);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {repositoryWithBranch}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public void CloneAllBranches(ILocalStoragePathFactory pathFormatter, GithubRepository githubRepository)
    {
        try
        {
            _fetcher.CloneAllBranches(pathFormatter, githubRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }
}