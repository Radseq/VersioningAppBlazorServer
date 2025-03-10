using VersioningAppBlazorServer.Models;

namespace VersioningAppBlazorServer.Utils;

public static class Mappers
{
    public static ApplicationView MapApplication_to_ApplicationView(Application app)
    {
        return new ApplicationView()
        {
            Id = app.Id,
            Name = app.Name,
            Versions = app.Versions.ConvertAll(Copys.Copy)
        };
    }

}
