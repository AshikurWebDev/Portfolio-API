namespace Portfolio.Application.Common.Interfaces;

/// <summary>
/// Tells open copies of the website that public content changed, so they refetch it without a reload.
/// Implemented in the Api project with SignalR; the Application layer only knows this interface.
/// </summary>
public interface ISiteEvents
{
    /// <summary>
    /// Announces that one kind of content changed. Best effort: a failure is logged, never thrown,
    /// because the change itself is already saved.
    /// </summary>
    /// <param name="topic">One of the <see cref="SiteTopics"/> values.</param>
    /// <param name="ct">Cancellation token.</param>
    Task ContentChangedAsync(string topic, CancellationToken ct = default);
}
