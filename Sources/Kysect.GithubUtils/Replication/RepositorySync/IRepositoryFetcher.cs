﻿using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.Replication.RepositorySync.LocalStoragePathFactories;

namespace Kysect.GithubUtils.Replication.RepositorySync;

public interface IRepositoryFetcher
{
    string EnsureRepositoryUpdated(ILocalStoragePathFactory pathFormatter, GithubRepository githubRepository);
    string Checkout(ILocalStoragePathFactory pathFormatter, GithubRepository repository, string branch, bool directoryPerBranch = false);
    void CloneAllBranches(ILocalStoragePathFactory pathFormatter, GithubRepository githubRepository);
}