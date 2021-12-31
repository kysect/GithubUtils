using System.Runtime.CompilerServices;

namespace Kysect.GithubUtils.RepositoryDiscovering;

public sealed class GitHubRepositoryDiscoveryService : IRepositoryDiscoveryService
{
    private const string GitHubApiBaseUrl = "https://api.github.com";
    private const string GitHubAcceptJsonHeaderValue = "application/vnd.github.v3+json";
    private const int PageSize = 100;

    private readonly string _token;

    public GitHubRepositoryDiscoveryService(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new RepositoryDiscoveryConfigurationException("GitHub Authorization Token was not provided",
                new ArgumentNullException(nameof(token)));

        _token = token;
    }

    public async IAsyncEnumerable<RepositoryRecord> TryDiscover(
        string organization,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(organization))
            throw new RepositoryDiscoveryConfigurationException("GitHub Organization was not specified",
                new ArgumentNullException(nameof(organization)));

        using var client = new GithubHttpClient(_token);
        var currentPage = 1;
        var previousPageLength = PageSize;
        while (previousPageLength == PageSize)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var page = await client.GetOnePageOfRepositories(organization, currentPage++);
            previousPageLength = page.Length;
            foreach (var repo in page)
            {
                yield return repo.MapToRecord();
            }
        }
    }
}