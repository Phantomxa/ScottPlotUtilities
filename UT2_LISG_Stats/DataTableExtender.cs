using System.Data;
using System.Text;

namespace BreakRateT1
{
    public static class DataTableExtender
    {
        public static string PrintToString(this DataTable table, int rowstoview)
        {
            int count = 0;
            var builder = new StringBuilder();

            foreach (DataColumn column in table.Columns)
            {
                builder.Append(column.ColumnName + "\t");
            }

            builder.AppendLine("\n");

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    builder.Append(row[column] + "\t");
                }
                count++;
                if (count >= rowstoview) { break; }
                else { builder.AppendLine(""); }
            }

            builder.AppendLine("");

            return builder.ToString();
        }
        public static string GetColumnNamesAndTypes(this DataTable table)
        {
            if (table == null || table.Columns.Count == 0)
                return "The DataTable is empty or null.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Column Name\t:\tData Type");

            foreach (DataColumn column in table.Columns)
            {
                sb.AppendLine($"{column.ColumnName}\t:\t{column.DataType.Name}");
            }

            return sb.ToString();
        }
    }
}
