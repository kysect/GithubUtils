namespace Kysect.GithubUtils.OrganizationReplicator;

public class OrganizationReplicationHub
{
    private readonly string _rootDirectory;

    public OrganizationReplicationHub(string rootDirectory)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);

        _rootDirectory = rootDirectory;
        Directory.CreateDirectory(rootDirectory);
    }

    public bool TryAddOrganization(string organization)
    {
        ArgumentNullException.ThrowIfNull(organization);

        string organizationDirectoryPath = Path.Combine(_rootDirectory, organization);
        if (Directory.Exists(organizationDirectoryPath))
            return false;

        Directory.CreateDirectory(organizationDirectoryPath);
        return true;
    }

    public IReadOnlyCollection<string> GetOrganizationNames()
    {
        return Directory.EnumerateDirectories(_rootDirectory).ToList();
    }
}