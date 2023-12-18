namespace Kysect.GithubUtils.RepositoryDiscovering.Exceptions;

public class RepositoryDiscoveryGenericException : Exception
{
    public RepositoryDiscoveryGenericException()
    {
    }

    public RepositoryDiscoveryGenericException(string message)
        : base(message)
    {
    }

    public RepositoryDiscoveryGenericException(string message, Exception inner)
        : base(message, inner)
    {
    }
}