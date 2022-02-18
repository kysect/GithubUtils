using LibGit2Sharp;

namespace Kysect.GithubUtils.RepositorySync;

public class RepositoryFetchOptions
{
    public bool IgnoreMissedBranch { get; }
    public CheckoutOptions CheckoutOptions { get; }

    public RepositoryFetchOptions(bool ignoreMissedBranch, CheckoutOptions checkoutOptions)
    {
        IgnoreMissedBranch = ignoreMissedBranch;
        CheckoutOptions = checkoutOptions;
    }

    public RepositoryFetchOptions() : this(false, new CheckoutOptions())
    {
    }
}