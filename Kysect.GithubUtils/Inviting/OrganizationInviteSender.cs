using Kysect.GithubUtils.Tools.Extensions;
using Octokit;
using Octokit.Internal;
using Serilog;
using System.Globalization;

namespace Kysect.GithubUtils;

public class OrganizationInviteSender
{
    private readonly Credentials _credentials;
    private readonly GitHubClient _client;
    private readonly OrganizationMembershipUpdate _addOrUpdateRequest;
    private readonly string _clientName;

    public OrganizationInviteSender(string token, string clientName = "kysect")
        : this(new Credentials(token), clientName)
    {
    }

    public OrganizationInviteSender(Credentials credentials, string clientName = "kysect")
    {
        _clientName = clientName;
        _credentials = credentials;
        _client = new GitHubClient(new ProductHeaderValue(clientName))
        {
            Credentials = credentials
        };
        _addOrUpdateRequest = new OrganizationMembershipUpdate();
    }

    /// <summary>
    /// Github has limit 50 invites per day. So method call will fail after this limit.
    /// </summary>
    public async Task<InviteResult> Invite(string organizationName, IReadOnlyCollection<string> usernames)
    {
        Log.Information($"Start sending invites to organization {organizationName}. Invites count: {usernames.Count}");

        usernames = usernames.Select(u => u.ToLower()).ToList();
        
        HashSet<string> organizationUsers = await GetAlreadyAdded(organizationName);
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

        List<string> successInvites = new List<string>();
        List<string> failedInvites = new List<string>();
        Exception? exception = null;

        foreach (string username in notExpired)
        {
            if (exception is not null)
            {
                failedInvites.Add(username);
                continue;
            }
            
            Log.Debug($"Invite user {username} to organization {organizationName}");

            try
            {
                await _client.Organization.Member.AddOrUpdateOrganizationMembership(organizationName, username, _addOrUpdateRequest);
                successInvites.Add(username);
                Log.Debug($"User {username} invited successful");
            }
            catch (Exception e)
            {
                Log.Error($"Failed to invite user {username}. Other users will not invited.");

                exception = e;
                failedInvites.Add(username);
            }
        }

        return new InviteResult(
            successInvites,
            failedInvites,
            addedUser,
            invited,
            expired,
            exception);
    }

    private async Task<HashSet<string>> GetAlreadyAdded(string organizationName)
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