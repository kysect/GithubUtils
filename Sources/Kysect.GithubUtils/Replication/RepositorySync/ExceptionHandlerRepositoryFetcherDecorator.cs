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

    public bool CloneRepositoryIfNeed(string targetPath, GithubRepository githubRepository)
    {
        try
        {
            return _fetcher.CloneRepositoryIfNeed(targetPath, githubRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public bool Clone(string targetPath, GithubRepository githubRepository)
    {
        try
        {
            return _fetcher.Clone(targetPath, githubRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public void FetchAllBranches(string targetPath, GithubRepository githubRepository)
    {
        try
        {
            _fetcher.FetchAllBranches(targetPath, githubRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public void CheckoutBranch(string targetPath, GithubRepository githubRepository, string branch)
    {
        try
        {
            _fetcher.CheckoutBranch(targetPath, githubRepository, branch);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public string EnsureRepositoryUpdated(string targetPath, GithubRepository githubRepository)
    {
        try
        {
            return _fetcher.EnsureRepositoryUpdated(targetPath, githubRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public string Checkout(string targetPath, GithubRepository repository, string branch)
    {
        try
        {
            return _fetcher.Checkout(targetPath, repository, branch);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {repository}, Branch: {branch}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }

    public IReadOnlyCollection<string> GetAllRemoteBranches(string targetPath, GithubRepository githubRepository)
    {
        try
        {
            return _fetcher.GetAllRemoteBranches(targetPath, githubRepository);
        }
        catch (Exception e)
        {
            string message = $"Exception while updating {githubRepository}.";
            _logger.LogError($"{message} Error: {e.Message}");
            throw new GithubUtilsException(message, e);
        }
    }
}