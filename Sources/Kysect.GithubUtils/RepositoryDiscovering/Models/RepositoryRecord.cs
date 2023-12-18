namespace Kysect.GithubUtils.RepositoryDiscovering.Models;

public class RepositoryRecord
{
    /// <summary>
    /// Name of repository
    /// </summary>
    /// <remarks>GitHub organization name is omitted</remarks>
    public string Name { get; }

    /// <summary>
    /// Url for cloning using SSH
    /// </summary>
    public string SshCloneUrl { get; }

    public string HttpsCloneUrl { get; }

    /// <summary>
    /// Default branch name (i.e. master or main)
    /// </summary>
    public string DefaultBranchName { get; }

    public RepositoryRecord(string name,
        string sshCloneUrl,
        string httpsCloneUrl,
        string defaultBranchName = "master")
    {
        Name = name;
        SshCloneUrl = sshCloneUrl;
        HttpsCloneUrl = httpsCloneUrl;
        DefaultBranchName = defaultBranchName;
    }
}