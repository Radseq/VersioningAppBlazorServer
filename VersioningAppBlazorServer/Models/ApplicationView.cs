namespace VersioningAppBlazorServer.Models;

public class ApplicationView : ApplicationDTO
{
    public AppVersionDTO? SelectedAppVersion { get; set; }
}
