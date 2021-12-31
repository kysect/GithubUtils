using Kysect.GithubUtils;

var gitUser = string.Empty;
var token = string.Empty;
var repositoryFetcher = new RepositoryFetcher(new FakeFormatter(), gitUser, token);
repositoryFetcher.EnsureRepositoryUpdated("fredikats", "test");

public class FakeFormatter : IPathFormatter
{
    public string FormatFolderPath(string username, string repository)
    {
        return Path.Combine("repo", username, repository);
    }
}