namespace Kysect.GithubUtils;

public record InviteResult(
    IReadOnlyCollection<string> Success,
    IReadOnlyCollection<string> Failed,
    IReadOnlyCollection<string> AlreadyAdded,
    IReadOnlyCollection<string> AlreadyInvited,
    IReadOnlyCollection<string> WithExpiredInvites,
    Exception? Exception);