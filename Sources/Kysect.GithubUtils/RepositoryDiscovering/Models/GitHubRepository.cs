using System.Text.Json.Serialization;

namespace Kysect.GithubUtils.RepositoryDiscovering.Models;

// TODO: fix nullability
internal sealed class GitHubRepositoryDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("clone_url")]
    public string CloneUrl { get; set; } = null!;

    [JsonPropertyName("ssh_url")]
    public string SshUrl { get; set; } = null!;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = null!;

    [JsonPropertyName("updated_at")]
    public DateTime LastUpdate { get; set; }

    [JsonPropertyName("pushed_at")]
    public DateTime LastPush { get; set; }

    [JsonPropertyName("default_branch")]
    public string DefaultBranch { get; set; } = null!;

    public RepositoryRecord MapToRecord()
    {
        return new RepositoryRecord(Name, SshUrl, CloneUrl, DefaultBranch);
    }
}