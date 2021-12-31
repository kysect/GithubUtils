using System.Text.Json.Serialization;

namespace Kysect.GithubUtils.RepositoryDiscovering;

internal sealed class GitHubRepository
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("clone_url")]
    public string CloneUrl { get; set; }

    [JsonPropertyName("ssh_url")]
    public string SshUrl { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime LastUpdate { get; set; }

    [JsonPropertyName("pushed_at")]
    public DateTime LastPush { get; set; }

    [JsonPropertyName("default_branch")]
    public string DefaultBranch { get; set; }

    public RepositoryRecord MapToRecord()
        => new RepositoryRecord(Name, SshUrl, CloneUrl, DefaultBranch);
}