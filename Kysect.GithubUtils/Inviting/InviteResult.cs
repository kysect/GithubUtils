namespace Kysect.GithubUtils;

public record InviteResult(
    IReadOnlyCollection<UserInviteResult> InviteResults,
    IReadOnlyCollection<string> AlreadyAdded,
    IReadOnlyCollection<string> AlreadyInvited,
    IReadOnlyCollection<string> WithExpiredInvites,
    Exception? Exception);