using Kysect.GithubUtils;

var repositoryFetcher = new RepositoryFetcher(new FakeFormatter());
repositoryFetcher.EnsureRepositoryUpdated("fredikats", "fredikats");

public class FakeFormatter : IPathFormatter
{
    public string FormatFolderPath(string username, string repository)
    {
        return Path.Combine("repo", username, repository);
    }
}