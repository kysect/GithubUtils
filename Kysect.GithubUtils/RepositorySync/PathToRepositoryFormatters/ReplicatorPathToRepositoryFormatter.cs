using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.OrganizationReplicator;

namespace Kysect.GithubUtils.RepositorySync;

public class ReplicatorPathToRepositoryFormatter : IPathToRepositoryFormatter
{
    private readonly IOrganizationReplicatorPathProvider _pathProvider;

    public ReplicatorPathToRepositoryFormatter(IOrganizationReplicatorPathProvider pathProvider)
    {
        _pathProvider = pathProvider;
    }

    public string FormatFolderPath(GithubRepository githubRepository)
    {
        return _pathProvider.GetPathToRepository(githubRepository);
    }
}