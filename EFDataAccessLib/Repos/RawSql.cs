using System.Data.Common;
using System.Data;
using System.Text.Json;
using EFDataAccessLib.DataAccess;

namespace EFDataAccessLib.Repos;

public class RawSql : IRawSql
{
    private readonly ProgDbContext dbContext;

    public RawSql(ProgDbContext _dbContext)
    {
        dbContext = _dbContext;
    }

    private static List<Dictionary<string, object?>> ConvertToDictionary(DbDataReader reader)
    {
        var columns = new List<string>();
        var rows = new List<Dictionary<string, object?>>();

        for (var i = 0; i < reader.FieldCount; i++)
        {
            columns.Add(reader.GetName(i));
        }

        while (reader.Read())
        {
            var row = new Dictionary<string, object?>();
            foreach (var column in columns)
            {
                var value = reader[column];
                // Convert DBNull.Value to null
                row[column] = value == DBNull.Value ? null : value;
            }
            if (row != null)
                rows.Add(row);
        }

        return rows;
    }

    public async Task<string?> RawSqlAsync(string sql, KeyValuePair<string, object?>[] sqlParms)
    {
        DbConnection dbConnection = dbContext.DbConnection;

        using var command = dbConnection.CreateCommand();

        command.CommandText = sql;

        for (int i = 0; i < sqlParms.Length; i++)
        {
            IDbDataParameter parm = command.CreateParameter();
            parm.ParameterName = sqlParms[i].Key;
            //parm.DbType = DbType.String;
            parm.Value = sqlParms[i].Value;
            command.Parameters.Add(parm);
        }

        await dbConnection.OpenAsync();

        string? jsonResult = null;

        var dt = new DataTable();

        using var reader = await command.ExecuteReaderAsync();

        var rows = ConvertToDictionary(reader);

        try
        {
            jsonResult = JsonSerializer.Serialize(rows/*, Formatting.Indented*/);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            await dbConnection.CloseAsync();
        }

        return jsonResult;
    }

    public async Task<int> RawSqlCountAsync(string sql, KeyValuePair<string, object?>[] sqlParms)
    {
        DbConnection dbConnection = dbContext.DbConnection;

        using var command = dbConnection.CreateCommand();
        command.CommandText = sql;

        for (int i = 0; i < sqlParms.Length; i++)
        {
            IDbDataParameter parm = command.CreateParameter();
            parm.ParameterName = sqlParms[i].Key;
            //parm.DbType = DbType.String;
            parm.Value = sqlParms[i].Value;
            command.Parameters.Add(parm);
        }

        await dbConnection.OpenAsync();

        var obj = await command.ExecuteScalarAsync();

        if (obj != null && int.TryParse(obj.ToString(), out int intValue))
        {
            await dbConnection.CloseAsync();
            return intValue;
        }
        else
        {
            await dbConnection.CloseAsync();
            return -1;
        }
    }
}
