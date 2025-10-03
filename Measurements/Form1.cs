using ScottPlotUtilities;
using System.Data;

namespace Measurements
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            DateTime start_time = DateTime.Now - DateTime.Now.TimeOfDay + new TimeSpan(0, 0, 1);
            poisonDateTime1.Format = DateTimePickerFormat.Custom;
            poisonDateTime1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            poisonDateTime1.Text = start_time.ToString();

            DateTime start_time2 = DateTime.Now;
            poisonDateTime2.Format = DateTimePickerFormat.Custom;
            poisonDateTime2.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            poisonDateTime2.Text = start_time2.ToString();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            var count = 0;
            var preform_no = tbPreformno.Text;

            var connection = new MsSqlConnectionManager();

            var query = Queries.CoatingGeometry(preform_no);

            var dtCoatingData = connection.ConnectList(query);

            foreach (var dt in dtCoatingData)
            {
                var x = dt.AsEnumerable().Select(x => (double)x[0]).ToArray();
                var y = dt.AsEnumerable().Select(x => Convert.ToDouble(x[1])).ToArray();

                var xName = dt.Columns[0].ColumnName;
                var yName = dt.Columns[1].ColumnName;

                if (count <= 1) { ScottPlotGraph.CreateScatter(x, y, formsPlot1, yName); }
                else { ScottPlotGraph.CreateScatter(x, y, formsPlot1, yName, rightAxis: true); }

                count++;
            }

            formsPlot1.PerformAutoScale();
            formsPlot1.Refresh();
        }
    }
}
