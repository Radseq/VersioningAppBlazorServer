namespace VersioningAppBlazorServer.Models.UI;

public class DeleteAppVersion
{
    public ApplicationDTO App { get; set; } = new();
    public AppVersionDTO Version { get; set; } = new();
}
