namespace VersioningAppBlazorServer.Models;

public class ApplicationDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<AppVersionDTO> Versions { get; set; } = [];
}

