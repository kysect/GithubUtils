using Kysect.GithubUtils.Tools.Extensions;
using Octokit;
using Serilog;

namespace Kysect.GithubUtils;

public class OrganizationInviteSender
{
    private readonly GitHubClient _client;
    private readonly OrganizationMembershipUpdate _addOrUpdateRequest = new OrganizationMembershipUpdate();

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

    public OrganizationInviteSender(GitHubClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Github has limit 50 invites per day. So method call will fail after this limit.
    /// </summary>
    public async Task<InviteResult> Invite(string organizationName, IReadOnlyCollection<string> usernames)
    {
        Log.Information($"Start sending invites to organization {organizationName}. Invites count: {usernames.Count}");

        usernames = usernames.Select(u => u.ToLower()).ToList();
        
        HashSet<string> organizationUsers = await GetAlreadyAddedUsers(organizationName);
        HashSet<string> alreadyInvitedUsers = await GetAlreadyInvitedUsers(organizationName);
        HashSet<string> expiredInvites = await GetExpiredInvites(organizationName);

        (IReadOnlyCollection<string> addedUser, IReadOnlyCollection<string> notAddedUser) = usernames.SplitBy(s => organizationUsers.Contains(s));
        if (addedUser.Any())
        {
            Log.Information($"Skip {addedUser.Count} users that already added.");
            Log.Debug($"Added users: " + string.Join(", ", addedUser));
        }

        (IReadOnlyCollection<string> invited, IReadOnlyCollection<string> notInvited) = notAddedUser.SplitBy(u => alreadyInvitedUsers.Contains(u));
        if (invited.Any())
        {
            Log.Information($"Skip {invited.Count} users that already invited.");
            Log.Debug($"Invited users: " + string.Join(", ", invited));
        }

        (IReadOnlyCollection<string>? expired, IReadOnlyCollection<string>? notExpired) = notInvited.SplitBy(u => expiredInvites.Contains(u));
        if (expired.Any())
        {
            Log.Information($"Skip {expired.Count} users that has already expired.");
            Log.Debug($"Expired users: " + string.Join(", ", expired));
        }

        var inviteResults = new List<UserInviteResult>();
        Exception? forbiddenException = null;

        foreach (string username in notExpired)
        {
            if (forbiddenException is not null)
            {
                inviteResults.Add(new UserInviteResult(username, UserInviteResultType.Skipped, forbiddenException.Message));
                continue;
            }
            
            Log.Debug($"Invite user {username} to organization {organizationName}");

            try
            {
                await _client.Organization.Member.AddOrUpdateOrganizationMembership(organizationName, username,
                    _addOrUpdateRequest);
                inviteResults.Add(new UserInviteResult(username, UserInviteResultType.Success, Reason: null));
                Log.Debug($"User {username} invited successful");
            }
            catch (ForbiddenException ex)
            {
                Log.Error("Invitation limit of 50 users per day has been reached. Other users will not be invited.");

                forbiddenException = ex;
                inviteResults.Add(new UserInviteResult(username, UserInviteResultType.Skipped, Reason: ex.Message));
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to invite user {username}.");

                inviteResults.Add(new UserInviteResult(username, UserInviteResultType.Failed, Reason: ex.Message));
            }
        }

        return new InviteResult(inviteResults, addedUser, invited, expired, forbiddenException);
    }

    private async Task<HashSet<string>> GetAlreadyAddedUsers(string organizationName)
    {
        IReadOnlyList<User> users = await _client.Organization.Member.GetAll(organizationName);
        return users
            .Select(u => u.Login.ToLower())
            .ToHashSet();
    }

    private async Task<HashSet<string>> GetAlreadyInvitedUsers(string organizationName)
    {
        IReadOnlyList<OrganizationMembershipInvitation> users = await _client.Organization.Member.GetAllPendingInvitations(organizationName);
        return users
            .Select(u => u.Login.ToLower())
            .ToHashSet();
    }

    public async Task<HashSet<string>> GetExpiredInvites(string organizationName)
    {
        IReadOnlyList<OrganizationMembershipInvitation> failedInvitations = await _client.Organization.Member.GetAllFailedInvitations(organizationName);
        return failedInvitations
            .Select(u => u.Login.ToLower())
            .ToHashSet();
    }
}