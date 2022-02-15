namespace Kysect.GithubUtils;

public interface IGithubActivityProvider
{
    Dictionary<string, ActivityInfo> GetActivityInfo(IReadOnlyCollection<string> usernames, DateTime? from = null, DateTime? to = null);
}