﻿using Kysect.GithubUtils.Models;

namespace Kysect.GithubUtils.OrganizationReplicator;

public interface IOrganizationReplicatorPathProvider
{
    string GetPathToOrganizations();
    string GetPathToOrganization(string organization);
    string GetPathToRepository(string organization, string repository);
    string GetPathToOrganizationWithBranch(string organization, string branch);
    string GetPathToRepositoryWithBranch(string organization, string branch, string repository);
}

public static class OrganizationReplicatorPathProviderExtensions
{
    public static string GetPathToRepository(this IOrganizationReplicatorPathProvider pathProvider,GithubRepository githubRepository)
    {
        ArgumentNullException.ThrowIfNull(pathProvider);

        return pathProvider.GetPathToRepository(githubRepository.Owner, githubRepository.Name);
    }
}