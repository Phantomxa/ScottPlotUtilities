using System;
using System.Data;
using System.Data.Odbc;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace UT2_LISG_Stats;

class MySqlConnectionManager
{
    public readonly string _connectionString;
    private readonly string _towerPrefix = "GAE870RD1DRW";

    public MySqlConnectionManager(string server, string database, string user, string pass)
    {
        _connectionString = $"Server = {server}; Database = {database}; Uid = {user}; Pwd = {pass}";
    }
    public MySqlConnectionManager(int tower, string database, string user, string pass)
    {
        string server = _towerPrefix + tower.ToString();

        _connectionString = $"Server = {server}; Database = {database}; Uid = {user}; Pwd = {pass}";
    }

    public async Task<DataTable> ExecuteQueryAsync(string query)
    {
        DataTable dt = new DataTable();

        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (MySqlCommand command = new MySqlCommand(query, conn))
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    await adapter.FillAsync(dt);
                    Console.WriteLine("MySQL query executed!");
                }
            }
        }

        return dt;
    }
    public DataTable ExecuteQuery(string query)
    {
        DataTable dt = new DataTable();

        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();
            using (MySqlCommand command = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
        }

        return dt;
    }
}
