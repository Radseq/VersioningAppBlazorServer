namespace VersioningAppBlazorServer.Models.UI;

public static class ValidationRegex
{
    public const string APP_NAME_REGEX = @"^(?!\s)([A-Z_ ]{3,30})(?<!\s)$";
}
