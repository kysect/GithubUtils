using Kysect.GithubUtils.Contributions.ApiResponses;

namespace Kysect.GithubUtils.Contributions.ActivityProviders;

public interface IGithubActivityProvider
{
    Dictionary<string, ActivityInfo> GetActivityInfo(IReadOnlyCollection<string> usernames, DateTime? from = null, DateTime? to = null);
}