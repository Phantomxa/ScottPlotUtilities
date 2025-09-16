using System.Data;
using System.Data.SqlClient;
//using Microsoft.Data.SqlClient;

namespace UT2_LISG_Stats;

internal class MsSqlConnectionManager
{
    private string _connectionString;
    public MsSqlConnectionManager()
    {
        _connectionString = "Data Source = nordevsql01; Initial Catalog = drawdb; Integrated Security = true";
    }
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
    public List<DataTable> ConnectList(string queryString)
    {
        var server = Environment.GetEnvironmentVariable("AZURE_SERVER");
        var db = Environment.GetEnvironmentVariable("metrics_mart");
        var _connectionString = "Data Source = nordevsql01; Initial Catalog = drawdb; Integrated Security = true";

#pragma warning disable CS0618 // Type or member is obsolete
        using (SqlConnection connection = new(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new(queryString, connection))
            {
                command.CommandTimeout = 100;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<DataTable> tables = new List<DataTable>();

                    do
                    {
                        DataTable dt = new DataTable();

                        // Load schema before reading rows
                        dt.Load(reader);

                        tables.Add(dt);
                    } while (!reader.IsClosed && reader.NextResult()); // Check if reader is still open before moving to the next result set

                    return tables;

                }
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}