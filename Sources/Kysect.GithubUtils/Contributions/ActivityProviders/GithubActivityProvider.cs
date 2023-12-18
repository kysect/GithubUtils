using Kysect.CommonLib.BaseTypes.Extensions;
using Kysect.GithubUtils.Contributions.ApiResponses;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace Kysect.GithubUtils.Contributions.ActivityProviders;

public class GithubActivityProvider : IGithubActivityProvider, IDisposable
{
    private const string Url = "https://github-contributions.now.sh/api/v1/";

    private readonly HttpClient _client;
    private readonly bool _isParallel;
    private readonly bool _withRetry;
    private readonly ILogger _logger;

    public GithubActivityProvider(ILogger logger, bool isParallel = true, bool withRetry = false)
    {
        _logger = logger;
        _isParallel = isParallel;
        _withRetry = withRetry;
        _client = new HttpClient();
    }

    public Dictionary<string, ActivityInfo> GetActivityInfo(IReadOnlyCollection<string> usernames, DateTime? from = null, DateTime? to = null)
    {
        usernames.ThrowIfNull();

        _logger.LogInformation($"Start getting activity for users. Users count: {usernames.Count}, Parallel: {_isParallel}, With retry: {_withRetry}");

        if (_isParallel)
        {
            _logger.LogDebug("Use parallel processing for getting activity info.");
            if (_withRetry)
            {
                _logger.LogDebug("Use retry processing for getting activity info.");
                return GetInfoWithRetry(usernames, from, to);
            }

            //TODO: make async
            return usernames
                .AsParallel()
                .ToDictionary(username => username, username => GetActivityInfo(username, from, to).Result);
        }

        return usernames
            .ToDictionary(username => username, username => GetActivityInfo(username, from, to).Result);

    }

    private async Task<ActivityInfo> GetActivityInfo(string username, DateTime? from = null, DateTime? to = null)
    {
        _logger.LogDebug($"Request activity for {username}, from: {from?.ToShortDateString()}, to {to?.ToShortDateString()}");

        string response = await _client.GetStringAsync(Url + username);
        var activityInfo = JsonSerializer.Deserialize<ActivityInfo>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        activityInfo.ThrowIfNull();
        //TODO: fix nullability
        return activityInfo.FilterValues(from, to);
    }

    /// <summary>
    /// Sometimes result is empty. We will try to resend request to ensure that we cannot fetch data.
    /// </summary>
    /// <param name="usernames"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private Dictionary<string, ActivityInfo> GetInfoWithRetry(IReadOnlyCollection<string> usernames, DateTime? from = null, DateTime? to = null)
    {
        int tryCount = 5;

        Dictionary<string, ActivityInfo> result = new Dictionary<string, ActivityInfo>();

        for (int i = 0; i < tryCount && result.Count != usernames.Count; i++)
        {
            if (i != 0)
            {
                Debug.Print($"Elements for getting activity in queue left: {usernames.Count - result.Count}");
                Thread.Sleep(2000);
            }

            //FYI: it is some kind of hack. Sometimes we get zero values
            List<(string Username, ActivityInfo Result)> localResult = usernames
                .Where(u => !result.ContainsKey(u))
                .AsParallel()
                .Select(username => (username, GetActivityInfo(username, from, to).Result))
                .Where(r => r.Result.Total > 0)
                .ToList();

            localResult.ForEach(r => result[r.Username] = r.Result);
        }

        return result;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}