using Microsoft.AspNetCore.SignalR;

namespace Portfolio.Api.Realtime;

/// <summary>
/// SignalR hub at <c>/hubs/site</c>. Server-to-client only: the website listens for
/// <see cref="ContentChangedMethod"/> and refetches that content, so changes made in the admin
/// (or by an AI agent) show up without a page reload. Visitors cannot call anything on it.
/// </summary>
public sealed class SiteHub : Hub
{
    /// <summary>Client method the server calls, with one argument: the topic (see SiteTopics).</summary>
    public const string ContentChangedMethod = "contentChanged";

    /// <summary>URL path the hub is mapped to.</summary>
    public const string Path = "/hubs/site";
}
