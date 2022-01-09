using Octokit;
using Serilog;

namespace Kysect.GithubUtils;

public record InviteResult(IReadOnlyCollection<string> SuccessInvites, IReadOnlyCollection<string> FailedInvites, Exception? Exception);

public class OrganizationInviteSender
{
    private readonly GitHubClient _client;

    public OrganizationInviteSender(string token, string clientName = "kysect")
        : this(new Credentials(token), clientName)
    {
    }

    public OrganizationInviteSender(Credentials credentials, string clientName = "kysect")
    {
        _client = new GitHubClient(new ProductHeaderValue(clientName))
        {
            Credentials = credentials
        };
    }

    /// <summary>
    /// Github has limit 50 invites per day. So method call will fail after this limit.
    /// </summary>
    public async Task<InviteResult> Invite(string organizationName, IReadOnlyCollection<string> usernames)
    {
        Log.Information($"Start sending invites to organization {organizationName}. Invites count: {usernames.Count}");
        List<string> successInvites = new List<string>();
        List<string> failedInvites = new List<string>();
        Exception? exception = null;
        var addOrUpdateRequest = new OrganizationMembershipUpdate();

        foreach (string username in usernames)
        {
            if (exception is not null)
            {
                Log.Debug($"Skip inviting for {username} to organization {organizationName}. Previous invited failed.");
                failedInvites.Add(username);
                continue;
            }

            Log.Debug($"Invite user {username} to organization {organizationName}");

            try
            {
                await _client.Organization.Member.AddOrUpdateOrganizationMembership(organizationName, username, addOrUpdateRequest);
                successInvites.Add(username);
                Log.Debug($"User {username} invited successful");
            }
            catch (Exception e)
            {
                Log.Error($"Failed to invite user {username}");

                exception = e;
                failedInvites.Add(username);
            }
        }

        return new InviteResult(successInvites, failedInvites, exception);
    }
}