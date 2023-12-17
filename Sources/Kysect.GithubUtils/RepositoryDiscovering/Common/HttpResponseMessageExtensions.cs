using Kysect.CommonLib.BaseTypes.Extensions;
using System.Net;
using System.Text.Json;

namespace Kysect.GithubUtils.RepositoryDiscovering;

internal static class HttpResponseMessageExtensions
{
    public static async Task<TResponse> ProcessResponseMessage<TResponse>(
        this HttpResponseMessage httpResponseMessage) where TResponse : class
    {
        if (!httpResponseMessage.IsSuccessStatusCode)
            return await httpResponseMessage.ProcessErrorResponse<TResponse>();

        return await httpResponseMessage.ProcessResponseBody<TResponse>();
    }

    private static async Task<TResponse> ProcessResponseBody<TResponse>(
        this HttpResponseMessage httpResponseMessage) where TResponse : class
    {
        string content;
        try
        {
            content = await httpResponseMessage.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            throw new RepositoryDiscoveryGenericException("Unable to read GitHub API response body", e);
        }

        try
        {
            return JsonSerializer.Deserialize<TResponse>(content).ThrowIfNull();
        }
        catch (Exception e)
        {
            throw new RepositoryDiscoveryGenericException(
                "Unable to deserialize GitHub API response body", e);
        }
    }

    private static Task<TResponse> ProcessErrorResponse<TResponse>(
        this HttpResponseMessage httpResponse) where TResponse : class
    {
        throw (int)httpResponse.StatusCode switch
        {
            (int)HttpStatusCode.NotFound => new RepositoryDiscoveryGenericException(
                "Specified organization was not found"),
            (int)HttpStatusCode.Unauthorized => new RepositoryDiscoveryGenericException(
                "Invalid token was passed"),
            (int)HttpStatusCode.Forbidden => new RepositoryDiscoveryGenericException(
                "API rate limit exceeded"),
            (int)HttpStatusCode.BadRequest => new RepositoryDiscoveryGenericException(
                "Malformed request"),
            422 => new RepositoryDiscoveryGenericException("Unprocessable request"),
            _ => new RepositoryDiscoveryGenericException(
                $"Unable to process response due to an error, HTTP StatusCode: {httpResponse.StatusCode}")
        };
    }
}