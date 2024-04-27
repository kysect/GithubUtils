namespace Kysect.GithubUtils.Models;

public readonly record struct CustomRemoteGitRepository(string Name, string RemoteHttpsUrl) : IRemoteGitRepository
{
    public string GetGitHttpsUrl()
    {
        return RemoteHttpsUrl;
    }

    public override string ToString()
    {
        return Name;
    }
}