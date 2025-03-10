namespace VersioningAppBlazorServer.Models;

public class Application
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<AppVersion> Versions { get; set; } = [];
}

public class ApplicationView : Application
{
    public AppVersion? SelectedAppVersion { get; set; }
}

