namespace VersioningAppBlazorServer.Models.UI;

public class AppWithLastAndProductVersion : ApplicationDTO
{
    public AppVersionDTO? LastVersion { get; set; } = null;
    public AppVersionDTO? ProductionVersion { get; set; } = null;
}
