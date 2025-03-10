namespace VersioningAppBlazorServer.Models;

public class AppVersion
{
    public int Id { get; set; } = -1;
    public int AppId { get; set; } = -1;
    public int Major { get; set; } = 0;
    public int Minor { get; set; } = 0;
    public int Patch { get; set; } = 0;
    public string Version => $"{Major}.{Minor}.{Patch}";
    public List<AppCompatibility> Compatibilities { get; set; } = [];
}
