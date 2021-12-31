namespace Kysect.GithubUtils.RepositoryDiscovering;

public class RepositoryDiscoveryConfigurationException : RepositoryDiscoveryGenericException
{
    public RepositoryDiscoveryConfigurationException()
        : base("Failed to configure repository discovery service")
    {
    }

    public RepositoryDiscoveryConfigurationException(string message)
        : base(message)
    {
    }

    public RepositoryDiscoveryConfigurationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}