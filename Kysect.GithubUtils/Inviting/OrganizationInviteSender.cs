using Microsoft.Extensions.Logging;
using Octokit;

namespace Kysect.GithubUtils;

public class OrganizationInviteSender
{
    private readonly GitHubClient _client;
    private readonly OrganizationMembershipUpdate _addOrUpdateRequest = new OrganizationMembershipUpdate();
    private readonly ILogger _logger;

    public OrganizationInviteSender(string token, ILogger logger, string clientName = "kysect")
        : this(new Credentials(token), logger, clientName)
    {
    }

    public OrganizationInviteSender(Credentials credentials, ILogger logger, string clientName = "kysect")
    {
        _logger = logger;
        _client = new GitHubClient(new ProductHeaderValue(clientName))
        {
            Credentials = credentials
        };
    }

    public OrganizationInviteSender(GitHubClient client, ILogger logger)
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// Github has limit 50 invites per day. So method call will fail after this limit.
    /// </summary>
    public async Task<IReadOnlyCollection<UserInviteResult>> Invite(string organizationName, IReadOnlyCollection<string> usernames)
    {
        _logger.LogInformation($"Start sending invites to organization {organizationName}. Invites count: {usernames.Count}");

        usernames = usernames.Select(u => u.ToLower()).ToList();

        var inviteResults = new List<UserInviteResult>();

        inviteResults.AddRange(await GetAlreadyAddedUsers(organizationName));
        inviteResults.AddRange(await GetAlreadyInvitedUsers(organizationName));
        inviteResults.AddRange(await GetExpiredInvites(organizationName));

        if (inviteResults.Any(result => result.Result is UserInviteResultType.AlreadyAdded))
        {
            IReadOnlyCollection<UserInviteResult> alreadyAdded = inviteResults
                .Where(result => result.Result is UserInviteResultType.AlreadyAdded)
                .ToList();

            _logger.LogInformation($"Skip {alreadyAdded.Count} users that already added.");
            _logger.LogDebug("Added users: " + string.Join(", ", alreadyAdded));
        }

        if (inviteResults.Any(result => result.Result is UserInviteResultType.AlreadyInvited))
        {
            IReadOnlyCollection<UserInviteResult> alreadyInvited = inviteResults
                .Where(result => result.Result is UserInviteResultType.AlreadyInvited)
                .ToList();

            _logger.LogInformation($"Skip {alreadyInvited.Count} users that already invited.");
            _logger.LogDebug("Invited users: " + string.Join(", ", alreadyInvited));
        }

        if (inviteResults.Any(result => result.Result is UserInviteResultType.InvitationExpired))
        {
            IReadOnlyCollection<UserInviteResult> invitationExpired = inviteResults
                .Where(result => result.Result is UserInviteResultType.InvitationExpired)
                .ToList();

            _logger.LogInformation($"Skip {invitationExpired.Count} users that has already expired.");
            _logger.LogDebug("Expired users: " + string.Join(", ", invitationExpired));
        }

        var usersToInvite = new List<string>();


        foreach (string username in usernames)
        {
            if (inviteResults.Any(i => string.Equals(i.Username, username, StringComparison.InvariantCultureIgnoreCase)))
                continue;

            usersToInvite.Add(username);
        }

        Exception? forbiddenException = null;

        foreach (string username in usersToInvite)
        {
            if (forbiddenException is not null)
            {
                inviteResults.Add(new UserInviteResult(username, UserInviteResultType.Skipped, forbiddenException.Message));
                continue;
            }
            
            _logger.LogDebug($"Invite user {username} to organization {organizationName}");

            try
            {
                await _client.Organization.Member.AddOrUpdateOrganizationMembership(organizationName, username,
                    _addOrUpdateRequest);
                inviteResults.Add(new UserInviteResult(username, UserInviteResultType.Success, Reason: null));
                _logger.LogDebug($"User {username} invited successful");
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError("Invitation limit of 50 users per day has been reached. Other users will not be invited.");

                forbiddenException = ex;
                inviteResults.Add(new UserInviteResult(username, UserInviteResultType.Skipped, Reason: forbiddenException.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to invite user {username}.");

                inviteResults.Add(new UserInviteResult(username, UserInviteResultType.Failed, Reason: ex.Message));
            }
        }

        return inviteResults;
    }

    private async Task<IReadOnlyCollection<UserInviteResult>> GetAlreadyAddedUsers(string organizationName)
    {
        IReadOnlyList<User> users = await _client.Organization.Member.GetAll(organizationName);
        return users
            .Select(user => new UserInviteResult(user.Login.ToLower(), UserInviteResultType.AlreadyAdded, Reason: null))
            .ToList();
    }

    private async Task<IReadOnlyCollection<UserInviteResult>> GetAlreadyInvitedUsers(string organizationName)
    {
        IReadOnlyList<OrganizationMembershipInvitation> users = await _client.Organization.Member.GetAllPendingInvitations(organizationName);
        return users
            .Select(user => new UserInviteResult(user.Login.ToLower(), UserInviteResultType.AlreadyInvited, Reason: null))
            .ToList();
    }

    public async Task<IReadOnlyCollection<UserInviteResult>> GetExpiredInvites(string organizationName)
    {
        IReadOnlyList<OrganizationMembershipInvitation> failedInvitations = await _client.Organization.Member.GetAllFailedInvitations(organizationName);
        return failedInvitations
            .Select(user => new UserInviteResult(user.Login.ToLower(), UserInviteResultType.InvitationExpired, Reason: null))
            .ToList();
    }
}