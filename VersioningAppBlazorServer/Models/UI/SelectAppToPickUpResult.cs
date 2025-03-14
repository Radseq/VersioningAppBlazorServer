namespace VersioningAppBlazorServer.Models.UI;

public class SelectAppToPickUpResult
{
    public SelectAppToPickUpResult(ApplicationDTO _Application, int _versionId)
    {
        Application = _Application;
        VersionId = _versionId;
    }
    public ApplicationDTO Application { get; set; }
    public int VersionId { get; set; } = -1;
}
