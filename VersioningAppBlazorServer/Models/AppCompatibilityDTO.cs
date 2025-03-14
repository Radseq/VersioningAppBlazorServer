namespace VersioningAppBlazorServer.Models;

public class AppCompatibilityDTO
{
    public int Id { get; set; } = -1;

    public int VersionId { get; set; } = -1;

    public int CompatibleWithVersionId { get; set; } = -1;
}
