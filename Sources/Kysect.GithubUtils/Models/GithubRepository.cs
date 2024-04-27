namespace Kysect.GithubUtils.Models;

public readonly record struct GithubRepository(string Owner, string Name) : IRemoteGitRepository
{
    public string GetGitHttpsUrl()
    {
        return $"https://github.com/{Owner}/{Name}.git";
    }

    public override string ToString()
    {
        return $"{Owner}/{Name}";
    }
}