using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace Kysect.GithubUtils.Replication.RepositorySync;

public class RepositoryFetchOptions
{
    public bool IgnoreMissedBranch { get; init; }
    public CheckoutOptions CheckoutOptions { get; init; }
    public FetchOptions FetchOptions { get; init; }
    public CloneOptions CloneOptions { get; init; }

    public RepositoryFetchOptions(bool ignoreMissedBranch, CheckoutOptions checkoutOptions, FetchOptions fetchOptions, CloneOptions cloneOptions)
    {
        IgnoreMissedBranch = ignoreMissedBranch;
        CheckoutOptions = checkoutOptions;
        FetchOptions = fetchOptions;
        CloneOptions = cloneOptions;
    }

    public RepositoryFetchOptions(string gitUser, string token, bool ignoreMissedBranch = false)
    {
        IgnoreMissedBranch = ignoreMissedBranch;
        CheckoutOptions = CreateDefaultCheckoutOptions();
        FetchOptions = CreateDefaultFetchOptions(gitUser, token);
        CloneOptions = CreateDefaultCloneOptions(gitUser, token);
    }

    public static CheckoutOptions CreateDefaultCheckoutOptions()
    {
        return new CheckoutOptions();
    }

    public static FetchOptions CreateDefaultFetchOptions(string gitUser, string token)
    {
        return new FetchOptions { CredentialsProvider = CreateHandlerForToken(gitUser, token) };
    }

    public static CloneOptions CreateDefaultCloneOptions(string gitUser, string token)
    {
        return new CloneOptions { CredentialsProvider = CreateHandlerForToken(gitUser, token) };
    }

    private static CredentialsHandler CreateHandlerForToken(string gitUser, string token)
    {
        return CreateCredentialsProvider;

        UsernamePasswordCredentials CreateCredentialsProvider(string url, string usernameFromUrl, SupportedCredentialTypes types)
        {
            return new UsernamePasswordCredentials { Username = gitUser, Password = token };
        }
    }
}