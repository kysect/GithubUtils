﻿using Kysect.GithubUtils.RepositoryDiscovering;
using Kysect.GithubUtils.RepositorySync;

namespace Kysect.GithubUtils.OrganizationReplicator;

public class OrganizationReplicationHub
{
    private readonly IOrganizationReplicatorPathProvider _pathProvider;

    public OrganizationReplicationHub(IOrganizationReplicatorPathProvider pathProvider)
    {
        ArgumentNullException.ThrowIfNull(pathProvider);

        _pathProvider = pathProvider;
    }

    public bool TryAddOrganization(string organization)
    {
        ArgumentNullException.ThrowIfNull(organization);

        string organizationDirectoryPath = _pathProvider.GetPathToOrganization(organization);
        if (Directory.Exists(organizationDirectoryPath))
            return false;

        Directory.CreateDirectory(organizationDirectoryPath);
        return true;
    }

    public IReadOnlyCollection<string> GetOrganizationNames()
    {
        string pathToOrganizations = _pathProvider.GetPathToOrganizations();
        Directory.CreateDirectory(pathToOrganizations);
        return Directory.EnumerateDirectories(pathToOrganizations).ToList();
    }

    public void SyncOrganizations(IRepositoryDiscoveryService discoveryService, RepositoryFetcher repositoryFetcher)
    {
        var pathFormatter = new ReplicatorPathToRepositoryFormatter(_pathProvider);
        var organizationFetcher = new OrganizationFetcher(discoveryService, repositoryFetcher, pathFormatter);

        foreach (string organizationName in GetOrganizationNames())
        {
            organizationFetcher.Fetch(organizationName);
        }
    }
}