using System.Data;

namespace UT2_LISG_Stats;

public static class Logger
{
    public static void WriteToFile(string message)
    {
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filepath = path + "\\ServiceLog_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".txt";
        if (!File.Exists(filepath))
        {
            using (StreamWriter writer = File.CreateText(filepath))
            {
                writer.WriteLine(message);
            }
        }
        else
        {
            using (StreamWriter writer = File.AppendText(filepath))
            {
                writer.WriteLine(message);
            }
        }
    }
    public static void WriteToQueryFile(string message)
    {
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filepath = path + "\\QueryLog_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".txt";
        if (!File.Exists(filepath))
        {
            using (StreamWriter writer = File.CreateText(filepath))
            {
                writer.WriteLine(message);
            }
        }
        else
        {
            using (StreamWriter writer = File.AppendText(filepath))
            {
                writer.WriteLine(message);
            }
        }
    }
    public static void WriteDataTable(DataTable table, int rowstoview)
    {
        int count = 0;
        foreach (DataColumn column in table.Columns)
        {
            WriteToFile(column.ColumnName + "\t");
        }

        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn column in table.Columns)
            {
                WriteToFile(row[column] + "\t");
            }
            count++;
            if (count >= rowstoview) { break; }
            else { WriteToFile("\n"); }
        }
        WriteToFile("\n");
    }
}
