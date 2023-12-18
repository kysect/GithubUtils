using Kysect.GithubUtils.RepositoryDiscovering.Exceptions;
using Kysect.GithubUtils.RepositoryDiscovering.Models;
using System.Net.Http.Headers;
using System.Reflection;

namespace Kysect.GithubUtils.RepositoryDiscovering.Common;

public class GithubHttpClient : IDisposable
{
    private const string GitHubApiBaseUrl = "https://api.github.com";
    private const string GitHubAcceptJsonHeaderValue = "application/vnd.github.v3+json";
    private const string ListOrganizationReposEndpointFormat = "/orgs/{0}/repos";
    private const int PageSize = 100;
    private readonly HttpClient _httpClient;

    public GithubHttpClient(string token)
    {
        string version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? throw new Exception("Failed to get assembly version.");
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(GitHubApiBaseUrl, UriKind.Absolute),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("token", token),
                Accept =
                {
                    new MediaTypeWithQualityHeaderValue(GitHubAcceptJsonHeaderValue)
                },
                UserAgent =
                {
                    new ProductInfoHeaderValue("DetCheckR", version)
                }
            },
            // GitHub limit for each API call is 10 seconds
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    internal async Task<GitHubRepositoryDto[]> GetOnePageOfRepositories(
        string organization,
        int currentPage,
        GitHubRepositoryType repositoryTypeFilter = GitHubRepositoryType.All)
    {
        string endpoint = BuildRepositoryListingEndpoint(organization, currentPage, repositoryTypeFilter);
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await _httpClient.GetAsync(endpoint);
        }
        catch (TaskCanceledException e) when (e.InnerException is TimeoutException)
        {
            // If GitHub takes more than 10 seconds to process an API request,
            // request will be terminated and we will receive a timeout response.
            throw new RepositoryDiscoveryGenericException(
                "GitHub was unable to process request in desired time limit and aborted the request", e);
        }
        catch (Exception e)
        {
            throw new RepositoryDiscoveryGenericException(
                "Network error occured during web request", e);
        }

        return await responseMessage.ProcessResponseMessage<GitHubRepositoryDto[]>();
    }

    private static string BuildRepositoryListingEndpoint(string organization,
        int page = 1,
        GitHubRepositoryType repositoryTypeFilter = GitHubRepositoryType.All)
    {
        if (string.IsNullOrEmpty(organization))
        {
            throw new RepositoryDiscoveryConfigurationException("GitHub Organization was not specified",
                new ArgumentNullException(nameof(organization)));
        }

        if (page < 1)
        {
            throw new RepositoryDiscoveryConfigurationException("Page number should be greater than zero",
                new ArgumentOutOfRangeException(nameof(page)));
        }


        var queryParameters = new Dictionary<string, string>
        {
            { "per_page", PageSize.ToString() },
            { "page", page.ToString() },
            { "type", repositoryTypeFilter.ToString("G") }
        };

        string path = string.Format(ListOrganizationReposEndpointFormat, organization);
        string query = ToQueryString(queryParameters);
        return string.Join("?", path, query);
    }

    private static string ToQueryString(Dictionary<string, string> queryParameters)
    {
        return string.Join("&", queryParameters.Select(ToQueryParameter));
    }

    private static string ToQueryParameter(KeyValuePair<string, string> kvp)
    {
        return string.Join("=", kvp.Key, kvp.Value);
    }
}