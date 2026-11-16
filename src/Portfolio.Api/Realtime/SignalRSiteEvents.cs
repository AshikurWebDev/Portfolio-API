using Microsoft.AspNetCore.SignalR;
using Portfolio.Application.Common.Interfaces;

namespace Portfolio.Api.Realtime;

/// <summary>
/// <see cref="ISiteEvents"/> over SignalR: broadcasts the changed topic to every connected website.
/// The message carries only the topic name, never content, so it reveals nothing that is not public.
/// </summary>
public sealed class SignalRSiteEvents(IHubContext<SiteHub> hub, ILogger<SignalRSiteEvents> log) : ISiteEvents
{
    /// <inheritdoc />
    public async Task ContentChangedAsync(string topic, CancellationToken ct = default)
    {
        try
        {
            await hub.Clients.All.SendAsync(SiteHub.ContentChangedMethod, topic, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // The change is already saved; a failed broadcast only means open pages refresh later.
            log.LogWarning(ex, "Could not broadcast a {Topic} change to open pages.", topic);
        }
    }
}
