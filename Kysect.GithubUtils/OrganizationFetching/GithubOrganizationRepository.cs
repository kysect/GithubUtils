namespace Kysect.GithubUtils.OrganizationFetching;

public class GithubOrganizationRepository
{
    public string Path { get; set; }
    public string OrganizationName { get; set; }
    public string RepositoryName { get; set; }

    public GithubOrganizationRepository(string path, string organizationName, string repositoryName)
    {
        Path = path;
        OrganizationName = organizationName;
        RepositoryName = repositoryName;
    }
}