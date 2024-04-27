using FluentAssertions;
using Kysect.CommonLib.Testing;
using Kysect.GithubUtils.Models;
using Kysect.GithubUtils.Replication.RepositorySync;
using System.IO.Abstractions;

namespace Kysect.GithubUtils.Tests.Replication.RepositorySync;

public class RepositoryFetcherTests : IDisposable
{
    private readonly TestTemporaryDirectory _temporaryDirectory;
    private readonly RepositoryFetcher _repositoryFetcher;
    private readonly FileSystem _fileSystem;

    public RepositoryFetcherTests()
    {
        _fileSystem = new FileSystem();
        _temporaryDirectory = new TestTemporaryDirectory(_fileSystem);
        _repositoryFetcher = new RepositoryFetcher(new RepositoryFetchOptions("test-user", "token"), TestLoggerProvider.Provide());
    }

    [Fact]
    public void Clone_RemoteGithubRepositoryByHttps_GitDirectoryCreated()
    {
        string targetPath = _temporaryDirectory.GetTemporaryDirectory();
        _repositoryFetcher.Clone(targetPath, new CustomRemoteGitRepository("GithubUtils", "https://github.com/kysect/GithubUtils.git"));

        string gitDirectoryPath = _fileSystem.Path.Combine(targetPath, ".git");
        _fileSystem.Directory.Exists(gitDirectoryPath).Should().BeTrue();
    }

    public void Dispose()
    {
        _temporaryDirectory.Dispose();
    }
}