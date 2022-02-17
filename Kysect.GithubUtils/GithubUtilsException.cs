namespace Kysect.GithubUtils;

public class GithubUtilsException : Exception
{
    public GithubUtilsException(string message, Exception exception) : base(message, exception)
    {
    }
}