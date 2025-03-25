using System.Reflection;

namespace VersioningAppBlazorServer.Models;

/// <summary>
/// A list of error descriptions returned as general errors from the API.
/// It consists of two partial classes - one for defining constants and one for code logic.
/// </summary>
public partial class ErrorList
{
    /// <summary>
    /// We need to systematize this, e.g., from 100 to 200, etc.
    /// </summary>

    // 100 system
    public const string _1 = "1";


    // 200 application
    public const string _201 = "201 Failed to add application";
    public const string _202 = "202 Failed to retrieve application (id)";
    public const string _203 = "203 Failed to add application, name is already taken";
    public const string _204 = "204 Failed to add application, name is empty";
    public const string _205 = "205 Failed to delete application";
    public const string _206 = "206 Failed to retrieve all applications";
    public const string _207 = "207 Failed to retrieve names of all applications";

    // 300 application versions
    public const string _301 = "301 Failed to add version to application";
    public const string _302 = "302 Failed to find application version (id)";
    public const string _303 = "303 Failed to delete application version";
    public const string _304 = "304 Failed to retrieve all application versions";
    public const string _305 = "305 Failed to download all changelogs for the given version.";
    public const string _306 = "306 Failed to set production flag";

    // 400 application version compatibilities
    public const string _401 = "401 Failed to add a compatible version to the specified version";
    public const string _402 = "402 Failed to remove compatibility between two versions";
    public const string _403 = "403 Failed to find compatibility between two versions";
    public const string _404 = "404 Failed to find compatibility for the specified application version";
    public const string _405 = "405 Failed to find the old compatibility version.";
    public const string _406 = "406 Failed to find the new compatibility version";
    public const string _407 = "407 Failed to update the old compatibility version to the new one.";
}

public partial class ErrorList : IErrorList
{
    private readonly Dictionary<int, string> dictionary = [];

    public string GetDescForErrorNr(int errNr)
    {
        return dictionary.TryGetValue(errNr, out var res) ? res : "Unknown error";
    }

    public ErrorList()
    {
        var list = GetConstants(typeof(ErrorList));
        foreach (var item in list)
        {
            dictionary.Add(ConvertNameToInt(item.Name), item.GetRawConstantValue()?.ToString() ?? string.Empty);
        }
    }

    private static List<FieldInfo> GetConstants(Type type)
    {
        FieldInfo[] fieldInfo = type.GetFields(BindingFlags.Public |
                                                BindingFlags.Static | BindingFlags.FlattenHierarchy);

        return fieldInfo.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
    }

    private static int ConvertNameToInt(string name)
    {
        var res = name.Replace("_", "");
        var parsed = int.TryParse(res, out var convertRes);
        if (!parsed)
            return 0;

        return convertRes;
    }
}

public interface IErrorList
{
    string GetDescForErrorNr(int errNr);
}