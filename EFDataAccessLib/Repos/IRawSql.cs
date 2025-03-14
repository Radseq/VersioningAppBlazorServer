namespace EFDataAccessLib.Repos;

public interface IRawSql
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="sqlParms"></param>
    /// <returns>json object of sql select</returns>
    Task<string?> RawSqlAsync(string sql, KeyValuePair<string, object?>[] sqlParms);

    Task<int> RawSqlCountAsync(string sql, KeyValuePair<string, object?>[] sqlParms);
}
