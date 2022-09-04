namespace Kysect.GithubUtils;

public record UserInviteResult(string Username, UserInviteResultType Result, string? Reason);

public enum UserInviteResultType
{
    Success = 1,
    Failed,
    Skipped,
    AlreadyAdded,
    AlreadyInvited,
    InvitationExpired,
}