using LibGit2Sharp;
using System.Text;

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

    public static RepositoryFetchOptions CreateWithUserPasswordAuth(string gitUser, string token, bool ignoreMissedBranch = false)
    {
        UsernamePasswordCredentials CreateCredentialsProvider(string url, string usernameFromUrl, SupportedCredentialTypes types)
        {
            return new UsernamePasswordCredentials { Username = gitUser, Password = token };
        }

        return new RepositoryFetchOptions(
            ignoreMissedBranch,
            new CheckoutOptions(),
            new FetchOptions { CredentialsProvider = CreateCredentialsProvider },
            new CloneOptions
            {
                FetchOptions =
                {
                    CredentialsProvider = CreateCredentialsProvider
                }
            }
        );
    }

    public static RepositoryFetchOptions CreateHeaderBasedAuth(string token, bool ignoreMissedBranch = false)
    {
        byte[] byteArray = Encoding.ASCII.GetBytes(":" + token);
        string encodedToken = Convert.ToBase64String(byteArray);

        string[] customHeaders = new[]
        {
            $"Authorization: Basic {encodedToken}"
        };

        return new RepositoryFetchOptions(
            ignoreMissedBranch,
            new CheckoutOptions(),
            new FetchOptions { CustomHeaders = customHeaders },
            new CloneOptions
            {
                FetchOptions =
                {
                    CustomHeaders = customHeaders
                }
            }
        );
    }
}