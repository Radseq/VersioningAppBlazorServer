using System.Text.RegularExpressions;

namespace VersioningAppBlazorServer.Utils;

public static class Formatters
{
    public static string FormatUrls(string input)
    {
        var urlRegex = new Regex(@"(http[s]?://[^\s]+)", RegexOptions.IgnoreCase);
        return urlRegex.Replace(input, match =>
        {
            var url = match.Value;
            return $"<a href='{url}' target='_blank' rel='noopener noreferrer'>{url}</a>";
        });
    }

}
