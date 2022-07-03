# GithubUtils

Some code from our project that we use for working with git and Github.

## Startup configuration sample:

```csharp
using Kysect.GithubUtils.RepositoryDiscovering;
using Kysect.GithubUtils.RepositorySync;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var discoveryService = new GitHubRepositoryDiscoveryService("github_token");
var repositoryFetchOptions = new RepositoryFetchOptions("github_username", "github_token");
var repositoryFetcher = new RepositoryFetcher(repositoryFetchOptions);
var organizationFetcher = new OrganizationFetcher(discoveryService, repositoryFetcher, new UseRepoForFolderNameStrategy(@"root_dir_path"));

var result = organizationFetcher.Fetch("github_org_name");
```

### Note:

Result in the line

```csharp
var result = organizationFetcher.Fetch("github_org_name");
```

stands only for cloned repositories infos, there is no action required to save something from this info to achieve repositories clones on your file system. Look for cloned repos by path specified as "root_dir_path".
