using System.Data.Odbc;
using System.Data;

namespace UT2_LISG_Stats;

class SybaseConnect
{
    public DataTable ConnectDBNew(string query)
    {
        var _connectionString = "Driver={SYBASE ASE ODBC Driver};Srvr=sybdev2;Uid=bautistaje;Pwd=Happy@128;Db=dsdb;";
        var dt = new DataTable();

        try
        {
            using (OdbcConnection connection = new OdbcConnection(_connectionString))
            {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(query, connection))
                {
                    command.CommandTimeout = 200;
                    using (OdbcDataReader reader = command.ExecuteReader())
                    {
                        dt.Load(reader); // Use Load() to fill the DataTable
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }

        return dt;
    }
    public async Task<DataTable> ConnectDBAsync(string query)
    {
        string _connectionString = "Driver={SYBASE ASE ODBC Driver};Srvr=sybdev2;Uid=bautistaje;Pwd=Happy@128;Db=dsdb;";

        return await Task.Run(() =>
        {
            DataTable dt = new DataTable();

            using (OdbcConnection connection = new OdbcConnection(_connectionString))
            {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(query, connection))
                {
                    command.CommandTimeout = 120;
                    using (OdbcDataAdapter adapter = new OdbcDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            Console.WriteLine("Sybase query executed!");
            MessageBox.Show("SUCCESSFUL!");

            return dt;
        });
    }
    public DataTable ConnectDB(string query)
    {
        string _connectionString = "Driver={SYBASE ASE ODBC Driver};Srvr=sybdev2;Uid=bautistaje;Pwd=Happy@128;Db=dsdb;";

        DataTable dt = new DataTable();

        using (OdbcConnection connection = new OdbcConnection(_connectionString))
        {
            connection.Open();
            using (OdbcCommand command = new OdbcCommand(query, connection))
            {
                command.CommandTimeout = 200;
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(command))
                {
                    int rowsAdded = adapter.Fill(dt);
                    connection.Close();
                }
            }
        }
        MessageBox.Show("Sybase query executed!");

        return dt;
    }
}
