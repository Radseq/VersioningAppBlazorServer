using System.Text;

namespace EFDataAccessLib;

public static class SqlUtils
{
    public static bool SqlQueryHasAndAtEnd(string sql)
    {
        if (!string.IsNullOrEmpty(sql))
            return sql.Substring(sql.Length - 5).ToLower().Contains("and");
        return false;
    }
    public static bool SqlQueryHasWhereEnd(string sql)
    {
        if (!string.IsNullOrEmpty(sql))
            return sql.Substring(sql.Length - 6).ToLower().Contains("where");
        return false;
    }

    public static string ConvertManyToSqlIN_Clause(string parmName, List<KeyValuePair<string, object>> parms)
    {
        StringBuilder sb = new();

        if (parms.Count > 0)
        {
            for (int i = 0; i < parms.Count; i++)
            {
                sb.Append(parmName + i.ToString() + ",");
            }
        }

        return sb.ToString();
    }
}
