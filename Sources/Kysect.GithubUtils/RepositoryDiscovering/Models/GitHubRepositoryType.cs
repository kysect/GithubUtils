namespace Kysect.GithubUtils.RepositoryDiscovering.Models;

/// <summary>
/// Specifies the types of GitHub repository
/// </summary>
public enum GitHubRepositoryType
{
    Public,
    Private,
    Forks,
    Sources,
    Member,

    [Obsolete("The 'internal' value is not yet supported on GitHub side")]
    Internal,

    /// <summary>
    /// The 'All' value includes each value from above.
    /// </summary>
    All
}