using Kysect.GithubUtils.RepositoryDiscovering.Common;
using Kysect.GithubUtils.RepositoryDiscovering.Exceptions;
using Kysect.GithubUtils.RepositoryDiscovering.Models;
using System.Runtime.CompilerServices;

namespace Kysect.GithubUtils.RepositoryDiscovering;

public sealed class GitHubRepositoryDiscoveryService : IRepositoryDiscoveryService
{
    private const int PageSize = 100;

    private readonly string _token;

    public GitHubRepositoryDiscoveryService(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new RepositoryDiscoveryConfigurationException("GitHub Authorization Token was not provided",
                new ArgumentNullException(nameof(token)));
        }

        _token = token;
    }

    public async IAsyncEnumerable<RepositoryRecord> TryDiscover(
        string organization,
        GitHubRepositoryType repositoryTypeFilter = GitHubRepositoryType.All,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(organization))
        {
            throw new RepositoryDiscoveryConfigurationException("GitHub Organization was not specified",
                new ArgumentNullException(nameof(organization)));
        }

        using var client = new GithubHttpClient(_token);
        var currentPage = 1;
        var previousPageLength = PageSize;
        while (previousPageLength == PageSize)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var page = await client.GetOnePageOfRepositories(organization, currentPage++, repositoryTypeFilter);
            previousPageLength = page.Length;
            foreach (var repo in page)
            {
                yield return repo.MapToRecord();
            }
        }
    }
}