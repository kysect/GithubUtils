namespace Kysect.GithubUtils.RepositoryDiscovering;

public class RepositoryRecord {
    /// <summary>
    /// Name of repository
    /// </summary>
    /// <remarks>GitHub organization name is omitted</remarks>
    public string Name { get; }

    /// <summary>
    /// Url for cloning using SSH
    /// </summary>
    public string SshCloneUrl { get; }

    /// <summary>
    /// <see cref="DetCheckR.Common.CommonGithubReader"/>-compatible HTTPS url for cloning
    /// </summary>
    public string HttpsCloneUrl { get; }

    /// <summary>
    /// Default branch name (i.e. master or main)
    /// </summary>
    public string DefaultBranchName { get; }

    public RepositoryRecord(string name,
        string sshCloneUrl,
        string httpsCloneUrl,
        string defaultBranchName = "master") {
        Name = name;
        SshCloneUrl = sshCloneUrl;
        HttpsCloneUrl = httpsCloneUrl;
        DefaultBranchName = defaultBranchName;
    }
}