namespace VersioningAppBlazorServer.Models;

public class ApplicationPicked
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AppVersionDTO Version { get; set; } = new();
}
