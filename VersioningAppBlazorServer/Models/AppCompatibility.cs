namespace VersioningAppBlazorServer.Models;

public class AppCompatibility
{
    public int Id { get; set; } = -1;
    // id wersji aplikacji podanej w CompatibleWithAppId
    public int AppVersionId { get; set; } = -1;
    public int CompatibleWithAppId { get; set; } = -1;
}
