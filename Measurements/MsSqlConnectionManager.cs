using System.Data;
using System.Data.SqlClient;
//using Microsoft.Data.SqlClient;

namespace Measurements;

internal class MsSqlConnectionManager
{
    private string _connectionString;
    public MsSqlConnectionManager()
    {
        _connectionString = "Data Source = nordevsql01; Initial Catalog = drawdb; Integrated Security = true";
    }
    /// <summary>
    /// Use this if you want to return a NEW DataTable.
    /// </summary>
    /// <param name="queryString"></param>
    /// <returns></returns>
    public DataTable Connect(string queryString)
    {
        var dt = new DataTable();

#pragma warning disable CS0618 // Type or member is obsolete
        using (SqlConnection connection = new(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new(queryString, connection))
            {
                command.CommandTimeout = 100;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }
                    else
                    {
                        //Logger.WriteToFileLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return dt;
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
    /// <summary>
    /// Use this if you want to fill a global DataTable.
    /// </summary>
    /// <param name="queryString"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public DataTable Connect(string queryString, DataTable dt)
    {
        using (SqlConnection connection = new(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new(queryString, connection))
            {
                command.CommandTimeout = 100;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }
                    else
                    {
                        //Logger.WriteToFileLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return dt;
        }
    }
    public List<DataTable> ConnectList(string queryString)
    {
        var _connectionString = "Data Source = nordevsql01; Initial Catalog = drawdb; Integrated Security = true";

        using SqlConnection connection = new(_connectionString);
        connection.Open();

        using SqlCommand command = new(queryString, connection);
        command.CommandTimeout = 100;

        using SqlDataReader reader = command.ExecuteReader();
        List<DataTable> tables = new();

        while (!reader.IsClosed)
        {
            DataTable dt = new DataTable();
            dt.Load(reader); // This advances to the next result set internally
            tables.Add(dt);
        }

        return tables;
    }
}