namespace Kysect.GithubUtils;

public static class GithubActivityProviderExtensions
{
    public static ActivityInfo GetActivityInfo(this IGithubActivityProvider provider, string username, DateTime? from = null, DateTime? to = null)
    {
        Dictionary<string, ActivityInfo> result = provider.GetActivityInfo(new[] { username }, from, to);
        if (result.TryGetValue(username, out ActivityInfo? activityInfo))
            return activityInfo;
        throw new Exception($"Activity for user {username} was not parsed");
    }
}