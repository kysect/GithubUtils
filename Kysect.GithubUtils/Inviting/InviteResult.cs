namespace Kysect.GithubUtils;

public record InviteResult(
    IReadOnlyCollection<UserInviteResult> InviteResults,
    Exception? Exception);