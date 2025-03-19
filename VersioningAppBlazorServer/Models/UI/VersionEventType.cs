namespace VersioningAppBlazorServer.Models.UI;

public enum VersionEventType
{
    NAVIGATE = 0,
    UPGRADE,
    DOWNGRADE,
    DELETE_COMPATIBILITY,
    SHOW_CHANGELOGS
}
