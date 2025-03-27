namespace VersioningAppBlazorServer.Models;

public class AppVersionDTO
{
    public int Id { get; set; } = -1;
    public int AppId { get; set; } = -1;
    public int Major { get; set; } = 0;
    public int Minor { get; set; } = 0;
    public int Patch { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
    public bool IsProduction { get; set; } = false;

    public string Version => $"{Major}.{Minor}.{Patch} {(IsProduction ? "©" : "")}";
    public List<AppCompatibilityDTO> Compatibilities { get; set; } = [];
}
