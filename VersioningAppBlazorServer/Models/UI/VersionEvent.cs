namespace VersioningAppBlazorServer.Models.UI;

public class VersionEvent
{
    public int AppId { get; set; } = -1;
    public int VersionId { get; set; } = -1;
    public int? OldVersionId { get; set; } = null;
    public VersionEventType EventType { get; set; } = VersionEventType.NAVIGATE;
}
