namespace Kysect.GithubUtils;

public class GithubUtilsException : Exception
{
    public GithubUtilsException(string message, Exception exception) : base(message, exception)
    {
    }

    public GithubUtilsException()
    {
    }

    public GithubUtilsException(string message) : base(message)
    {
    }
}