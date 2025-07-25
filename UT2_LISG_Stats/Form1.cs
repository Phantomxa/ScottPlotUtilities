using ScottPlot;
using ScottPlot.Statistics;
using ScottPlot.WinForms;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Color = ScottPlot.Color;

namespace UT2_LISG_Stats;

public partial class Form1 : Form
{
    int counter = 0;
    int movingAverageWindowSize;
    int slopeWindowSize;
    public string clad;
    List<string> legendItems = new();
    Dictionary<string, IPlottable> seriesDict = new Dictionary<string, IPlottable>();
    Coordinates[] DataPoints;
    Coordinates MouseDownCoordinates;
    Coordinates MouseNowCoordinates;
    CoordinateRect MouseSlectionRect => new(MouseDownCoordinates, MouseNowCoordinates);
    bool MouseIsDown = false;
    public ScottPlot.Plottables.Rectangle RectanglePlot;
    public string Title => "Select Data Points";

    public string Description => "Demonstrates how to use mouse events " +
        "to draw a rectangle around data points to select them";

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

        formsPlot1.MouseMove += FormsPlot1_MouseMove;
        formsPlot1.MouseDown += FormsPlot1_MouseDown;
        formsPlot1.MouseUp += FormsPlot1_MouseUp;

        // add a rectangle we can use as a selection indicator
        RectanglePlot = formsPlot1.Plot.Add.Rectangle(0, 0, 0, 0);
        RectanglePlot.FillStyle.Color = Colors.Red.WithAlpha(.2);
        RectanglePlot.Axes.YAxis = formsPlot1.Plot.Axes.Right;

        movingAverageWindowSize = int.Parse(tbMovingAverage.Text);
        slopeWindowSize = int.Parse(tbSlope.Text);
    }

    private void tbTower1_Click(object sender, EventArgs e)
    {
        tbTower1.Text = "";
    }
    private void btnExecute_Click(object sender, EventArgs e)
    {
        ClearCharts();
        legendItems?.Clear();

        string db = "test";
        string uid = "ignition";
        //string uid = Environment.GetEnvironmentVariable("TOWER_MYSQL_USER")!;
        //string pwd = Environment.GetEnvironmentVariable("TOWER_MYSQL_PASSWORD")!;
        string pwd = "ignition";

        string tower = tbMovingAverage.Text;
        movingAverageWindowSize = int.Parse(tbMovingAverage.Text);
        slopeWindowSize = int.Parse(tbSlope.Text);

        int.TryParse(tbTower1.Text, out int towerInt);

        if (towerInt >= 385)
        {
            clad = "Clad";
        }
        else
        {
            clad = "Cersa_Diam";
        }

        DateTime date;
        DateTime.TryParseExact(poisonDateTime1.Text, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        string new_date = date.ToString("yyyy-MM-dd HH:mm:ss");

        DateTime date2;
        DateTime.TryParseExact(poisonDateTime2.Text, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date2);
        string new_date2 = date2.ToString("yyyy-MM-dd HH:mm:ss");

        // Pull Feed Position and plot this to see when Feed 2 Torque Limit reached
        string query = $"select * from cersacladdata1 where t_stamp >= \"" + new_date + "\" AND t_stamp <= \"" + new_date2 + "\" AND Diam_Peak_Peak IS NOT NULL AND Cersa_Diam IS NOT NULL  AND Cersa_Xpos_mm IS NOT NULL AND Cersa_Ypos_mm IS NOT NULL order by t_stamp asc";
        string queryChillers = $"select ((LR_Fraction/100)*3) AS LR, ((BF_Fraction/100)*3) AS BF, t_stamp from chiller_position1 where t_stamp >= \"" + new_date + "\" AND t_stamp <= \"" + new_date2 + "\" AND LR_Fraction IS NOT NULL AND BF_Fraction IS NOT NULL order by t_stamp desc";
        string queryTension = $"select center/100 AS center, power/100 AS power, t_stamp from centvibpowdata1 where t_stamp >= \"" + new_date + "\" AND t_stamp <= \"" + new_date2 + "\" AND center IS NOT NULL AND power IS NOT NULL order by t_stamp desc";
        string queryLS = $"select LineSpeed, t_stamp from cladhist1 where t_stamp >= \"" + new_date + "\" AND t_stamp <= \"" + new_date2 + "\" AND LineSpeed IS NOT NULL AND t_stamp IS NOT NULL order by t_stamp desc";
        string queryCladDev = $"select CladDev, t_stamp from cladstatshist1 where t_stamp >= \"" + new_date + "\" AND t_stamp <= \"" + new_date2 + "\" AND CladDev IS NOT NULL AND t_stamp IS NOT NULL order by t_stamp desc";
        string queryCladUT4 = $"select LineSpeed, Clad, t_stamp from cladhist1 where t_stamp >= \"" + new_date + "\" AND t_stamp <= \"" + new_date2 + "\" AND LineSpeed IS NOT NULL AND t_stamp IS NOT NULL AND Clad IS NOT NULL order by t_stamp desc";

        string syb_QueryClad = $@"SELECT event_ts, mach_no, datum1, datum2, datum3, datum4, datum5 
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  mach_no = '{tower}' AND
                                    event_num = 30 AND 
                                    event_ts >= '{new_date}' AND
                                    event_ts <= '{new_date2}'
                                    order by event_ts asc";
        string syb_QueryBore = $@"SELECT event_ts, mach_no, datum1, datum2, datum3, datum4, datum5, event_num
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  mach_no = '{tower}' AND
                                    event_num in (116, 119) AND 
                                    event_ts >= '{new_date}' AND
                                    event_ts <= '{new_date2}'
                                    order by event_ts asc";
        string syb_AFC = $@"SELECT event_ts, datum3
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  mach_no = '{tower}' AND
                                    event_num in (130) AND 
                                    event_ts >= '{new_date}'AND
                                    event_ts <= '{new_date2}'
                                    order by event_ts asc";
        string syb_QueryCladAirlineExcursion =
                                $@"SELECT event_ts, event_num, mach_no, datum1, datum3 
                                    FROM dsdb..draw_event NOHOLDLOCK 
                                    WHERE  mach_no = '{tower}' AND
                                    event_num in (32,39) AND 
                                    event_ts >= '{new_date}' AND
                                    event_ts <= '{new_date2}'
                                    order by event_ts asc";

        MySqlConnectionManager connection = new MySqlConnectionManager(towerInt, db, uid!, pwd!);
        DataTable dt = new DataTable();
        DataTable dt_chiller = new DataTable();
        DataTable dt_tension = new DataTable();
        DataTable dt_LS = new DataTable();
        DataTable dt_CladDev = new DataTable();

        try
        {
            if (towerInt >= 385)
            {
                dt = connection.ExecuteQuery(queryCladUT4);
            }
            else
            {
                Console.WriteLine();
                dt = connection.ExecuteQuery(query);
            }
            // dt_chiller = connection.ExecuteQuery(queryChillers);
            // dt_tension = connection.ExecuteQuery(queryTension);
            // dt_LS = connection.ExecuteQuery(queryLS);
            // dt_CladDev = connection.ExecuteQuery(queryCladDev);
        }
        catch (Exception)
        {
            throw;
            //label1.Text = $"Error:\n {ex.Message}";
            //label1.ForeColor = Color.Red;
            //label1.Visible = true;
        }

        // SybaseConnect sybConnect = new SybaseConnect();
        // DataTable dtClad = sybConnect.ConnectDBNew(syb_QueryClad);
        // DataTable dtBore = sybConnect.ConnectDBNew(syb_QueryBore);
        // DataTable dtAFC = sybConnect.ConnectDBNew(syb_AFC);
        // DataTable dtCladAirline = sybConnect.ConnectDBNew(syb_QueryCladAirlineExcursion);
        // DataTable dtClad_avg;

        DateTime[] xValues = dt.AsEnumerable().Select(x => x.Field<DateTime>("t_stamp")).ToArray();
        double[] cladVals = dt.AsEnumerable().Select(x => (double)x.Field<float>(clad)).ToArray();
        double[] p2pVals = dt.AsEnumerable().Select(x => (double)x.Field<float>("Diam_Peak_Peak")).ToArray();
        double[] lr= dt.AsEnumerable().Select(x => (double)x.Field<float>("Cersa_Xpos_mm")).ToArray();
        double[] fb= dt.AsEnumerable().Select(x => (double)x.Field<float>("Cersa_Ypos_mm")).ToArray();

        CreateScottSignalXY(xValues, lr, formsPlot2, "LR+", seriesDict);
        CreateScottSignalXY(xValues, fb, formsPlot2, "FB+", seriesDict);
        CreateScottSignalXYSecondary(xValues, cladVals, formsPlot1, "CladDiam", seriesDict);
        CreateScottSignalXY(xValues, p2pVals, formsPlot1, "P2P", seriesDict);

        var plot1X = formsPlot1.Plot.Axes.Bottom;
        var plot2X = formsPlot2.Plot.Axes.Bottom;

        formsPlot1.Plot.Axes.Link(plot1X, plot2X, formsPlot2.Plot);
        formsPlot2.Plot.Axes.Link(plot2X, plot1X, formsPlot1.Plot);

        DataPoints = ConvertToCoordinates(xValues, p2pVals);

        Console.WriteLine();
    }

    public void ClearCharts()
    {
        formsPlot1.Reset();
        formsPlot2.Reset();
        formsPlot1.Refresh();
        formsPlot2.Refresh();
    }
    public static IEnumerable<T> Unique<T>(DataTable table, string columnName)
    {
        var uniqueValues = table.AsEnumerable().GroupBy(row => row.Field<T>(columnName)).Select(group => group.First().Field<T>(columnName));

        return uniqueValues;
    }
    private bool IsCorrectTower(string mach_no)
    {
        int tower;
        if (Int32.TryParse(mach_no, out tower))
        {
            if (tower < 397 & tower > 348)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public static void PrintValues(DataTable table, int rowstoview)
    {
        int count = 0;
        foreach (DataColumn column in table.Columns)
        {
            Console.Write(column.ColumnName + "\t");
        }

        Console.WriteLine();

        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn column in table.Columns)
            {
                Console.Write(row[column] + "\t");
            }
            count++;
            if (count >= rowstoview) { break; }
            else { Console.Write("\n"); }
        }
        Console.WriteLine();
    }
    public static void CreateScottScatter(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
        scatter.LegendText = legendName;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
    }
    public static void CreateScottSignalXY(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, Dictionary<string, IPlottable> dictionary)
    {
        var scatter = formsPlot.Plot.Add.SignalXY(xs, ydata);
        scatter.LegendText = legendName;
        dictionary[legendName] = scatter;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
    }
    public static void CreateScottScatter(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, System.Drawing.Color color)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
        scatter.LineColor = Color.FromColor(color);
        scatter.LegendText = legendName;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
    }
    public static void CreateScottSignalXY(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, System.Drawing.Color color)
    {
        var scatter = formsPlot.Plot.Add.SignalXY(xs, ydata);
        scatter.LineColor = Color.FromColor(color);
        scatter.LegendText = legendName;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
    }
    public static void CreateScottSignalXY(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, System.Drawing.Color color, Dictionary<string, IPlottable> dictionary)
    {
        var scatter = formsPlot.Plot.Add.SignalXY(xs, ydata);
        scatter.LineColor = Color.FromColor(color);
        scatter.LegendText = legendName;
        dictionary[legendName] = scatter;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
    }
    public static void CreateScottSignalXY(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName)
    {
        var scatter = formsPlot.Plot.Add.SignalXY(xs, ydata);
        scatter.LegendText = legendName;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
    }
    public static void CreateScottSignalXYSecondary(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, Dictionary<string, IPlottable> dictionary)
    {
        var scatter = formsPlot.Plot.Add.SignalXY(xs, ydata);
        scatter.Axes.YAxis = formsPlot.Plot.Axes.Right;
        scatter.LegendText = legendName;
        dictionary[legendName] = scatter;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
    }
    public static void CreateScottScatterNoLine(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
        scatter.LineWidth = 0;
        scatter.LegendText = legendName;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
    }
    public static void CreateScottScatterNoLine(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, Dictionary<string, IPlottable> dictionary)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
        scatter.LineWidth = 0;
        scatter.LegendText = legendName;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        dictionary[legendName] = scatter;
        formsPlot.Refresh();
    }
    public static void CreateScottSecondary(DateTime[] myDates, double[] ydata, FormsPlot formsPlot, string legendName)
    {
        //var yAxis3 = formsPlot.Plot.Axes.AddRightAxis(); // Do this if want a second Right axis (so 2 of them on the right)
        var sigBig = formsPlot.Plot.Add.Scatter(myDates, ydata);
        sigBig.Axes.YAxis = formsPlot.Plot.Axes.Right;
        sigBig.LegendText = legendName;
        formsPlot.Refresh();
    }
    public static void CreateScottSecondary(DateTime[] myDates, double[] ydata, FormsPlot formsPlot, string legendName, string secondaryAxisName)
    {
        //var yAxis3 = formsPlot.Plot.Axes.AddRightAxis(); // Do this if want a second Right axis (so 2 of them on the right)
        var sigBig = formsPlot.Plot.Add.Scatter(myDates, ydata);
        sigBig.Axes.YAxis = formsPlot.Plot.Axes.Right;
        sigBig.LegendText = legendName;
        formsPlot.Plot.Axes.Right.Label.Text = secondaryAxisName;
        formsPlot.Refresh();
    }
    public static void DarkMode(FormsPlot formsPlot)
    {
        // change figure colors
        formsPlot.Plot.FigureBackground.Color = Color.FromHex("#181818");
        formsPlot.Plot.DataBackground.Color = Color.FromHex("#1f1f1f");

        // change axis and grid colors
        formsPlot.Plot.Axes.Color(Color.FromHex("#d7d7d7"));
        formsPlot.Plot.Grid.MajorLineColor = Color.FromHex("#404040");

        // change legend colors
        formsPlot.Plot.Legend.BackgroundColor = Color.FromHex("#404040");
        formsPlot.Plot.Legend.FontColor = Color.FromHex("#d7d7d7");
        formsPlot.Plot.Legend.OutlineColor = Color.FromHex("#d7d7d7");
    }
    public DataTable GroupData(DataTable dt, string timeColumnName, string[] valueColumnNames)
    {
        int length = valueColumnNames.Length;

        DataTable groupedTable = new();
        groupedTable.Columns.Add("t_stamp", typeof(DateTime));

        foreach (var columnName in valueColumnNames)
        {
            groupedTable.Columns.Add($"{columnName}", typeof(float));
        }

        // Group by 10-second intervals and calculate the averages
        var groupedData = dt.AsEnumerable()
            .GroupBy(row =>
            {
                var timestamp = row.Field<DateTime>(timeColumnName);
                return new DateTime(
                    timestamp.Year,
                    timestamp.Month,
                    timestamp.Day,
                    timestamp.Hour,
                    timestamp.Minute,
                    (timestamp.Second / 10) * 10); // Group by 10-second intervals
            })
            .Select(group =>
            {
                var averages = valueColumnNames
                    .Select(columnName => group.Average(row => row.Field<float>(columnName)))
                    .ToArray();

                return new
                {
                    t_stamp = group.Key,
                    Averages = averages
                };
            });

        // Add the grouped data to the new DataTable
        foreach (var item in groupedData)
        {
            var rowValues = new object[valueColumnNames.Length + 1];
            rowValues[0] = item.t_stamp;
            item.Averages.CopyTo(rowValues, 1); // Copy averages to the row

            groupedTable.Rows.Add(rowValues);
        }

        return groupedTable;
    }
    public double Maximum(DataTable dt2, string column)
    {
        double max = (double)dt2.Compute($"MAX({column})", "");
        return max;
    }
    public double StandardDev(DataTable dt2, int factor, string direction, string column)
    {
        // Calculate the mean of the data points.
        double mean = (double)dt2.Compute($"AVG({column})", "");

        // Calculate the standard deviation of the data points.
        double stdDev = (double)dt2.Compute($"STDEV({column})", "");

        // Calculate the third standard deviation of the data points.
        double factoredStdDev = stdDev * factor;

        switch (direction)
        {
            case "+":
                // Calculate the result.
                double filteredValue = mean + factoredStdDev;
                return filteredValue;
            case "-":
                // Calculate the result.
                double filteredValue2 = mean - factoredStdDev;
                return filteredValue2;
            default:
                // Calculate the result.
                double filteredValue3 = mean - factoredStdDev;
                return filteredValue3;
        }

    }
    public void PrintDataTableToTextBox(DataTable table, TextBox textBox)
    {
        StringBuilder sb = new();

        // Print column names
        foreach (DataColumn column in table.Columns)
        {
            sb.Append(column.ColumnName + "\t");
        }
        sb.AppendLine();

        // Print row values
        foreach (DataRow row in table.Rows)
        {
            foreach (var item in row.ItemArray)
            {
                sb.Append(item?.ToString() + "\t");
            }
            sb.AppendLine();
        }

        // Output to the TextBox
        //textBox.Text = sb.ToString();
        textBox.AppendText(sb.ToString());
    }
    public void PrintColumnNamesAndTypes(DataTable table, TextBox textBox)
    {
        StringBuilder sb = new();

        // Print column names and types
        foreach (DataColumn column in table.Columns)
        {
            sb.AppendLine($"{column.ColumnName} ({column.DataType.Name})");
        }

        // Output to the TextBox
        textBox.Text = sb.ToString();
    }
    private void FormsPlot1_MouseDown(object? sender, MouseEventArgs e)
    {
        if (!cbSelectPoints.Checked)
            return;

        MouseIsDown = true;
        RectanglePlot.IsVisible = true;
        MouseDownCoordinates = formsPlot1.Plot.GetCoordinates(e.X, e.Y);
        formsPlot1.UserInputProcessor.Disable(); // disable the default click-drag-pan behavior
    }

    private void FormsPlot1_MouseUp(object? sender, MouseEventArgs e)
    {
        if (!cbSelectPoints.Checked)
            return;

        MouseIsDown = false;
        RectanglePlot.IsVisible = false;
        movingAverageWindowSize = int.Parse(tbMovingAverage.Text);
        slopeWindowSize = int.Parse(tbSlope.Text);

        // clear old markers
        formsPlot1.Plot.Remove<ScottPlot.Plottables.Marker>();

        if (counter > 0)
        {
            formsPlot1.Plot.Remove(seriesDict["Moving Average"]);
            formsPlot1.Plot.Remove(seriesDict["Slope"]);
            seriesDict.Remove("Moving Average");
            seriesDict.Remove("Slope"); 
        }

        // identify selectedPoints
        var selectedPoints = DataPoints.Where(x => MouseSlectionRect.Contains(x));
        var myTable = ConvertCoordinatesToDataTable(selectedPoints);

        if (cbShowPoints.Checked)
        {
            // add markers to outline selected points
            foreach (Coordinates selectedPoint in selectedPoints)
            {
                var newMarker = formsPlot1.Plot.Add.Marker(selectedPoint);
                newMarker.MarkerStyle.Shape = MarkerShape.OpenCircle;
                newMarker.MarkerStyle.Size = 10;
                newMarker.MarkerStyle.FillColor = Colors.Red.WithAlpha(.2);
                newMarker.MarkerStyle.LineColor = Colors.Red;
                newMarker.MarkerStyle.LineWidth = 1;
            }
        }
        
        TankStateAnalyzer.CalculateMovingAverage(myTable, new List<string> { "Y" }, movingAverageWindowSize);
        TankStateAnalyzer.CalculateSlope(myTable, new List<string> { "Y_MovingAvg" }, slopeWindowSize, "X");

        DateTime[] xCladValues = myTable.AsEnumerable().Select(x => DateTime.FromOADate(x.Field<double>("X"))).ToArray();
        double[] yCladValues = myTable.AsEnumerable().Select(x => (double)x.Field<float>("Y_MovingAvg")).ToArray();

        DateTime[] xSlopeValues = myTable.AsEnumerable().Select(x => DateTime.FromOADate(x.Field<double>("X"))).ToArray();
        double[] ySlopeValues = myTable.AsEnumerable().Select(x => (double)x.Field<float>("Y_Slope")).ToArray();

        CreateScottSignalXY(xCladValues, yCladValues, formsPlot1, "Moving Average", System.Drawing.Color.BlueViolet, seriesDict);
        CreateScottSignalXY(xSlopeValues, ySlopeValues, formsPlot1, "Slope", seriesDict);

        myTable.Reset();

        // reset the mouse positions
        MouseDownCoordinates = Coordinates.NaN;
        MouseNowCoordinates = Coordinates.NaN;

        // update the plot
        formsPlot1.Refresh();
        formsPlot1.UserInputProcessor.Enable(); // re-enable the default click-drag-pan behavior
        counter++;
    }

    private void FormsPlot1_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!MouseIsDown || !cbSelectPoints.Checked)
            return;

        MouseNowCoordinates = formsPlot1.Plot.GetCoordinates(e.X, e.Y);
        RectanglePlot.CoordinateRect = MouseSlectionRect;
        formsPlot1.Refresh();
    }
    public static Coordinates[] ConvertToCoordinates(DateTime[] xValues, double[] yValues)
    {
        if (xValues.Length != yValues.Length)
            throw new ArgumentException("xValues and yValues must be the same length.");

        Coordinates[] coords = new Coordinates[xValues.Length];
        for (int i = 0; i < xValues.Length; i++)
        {
            coords[i] = new Coordinates(xValues[i].ToOADate(), yValues[i]);
        }

        return coords;
    }
    public static DataTable ConvertCoordinatesToDataTable(IEnumerable<Coordinates> points)
    {
        DataTable table = new DataTable();

        table.Columns.Add("X", typeof(double));
        table.Columns.Add("Y", typeof(double));

        foreach (var pt in points)
        {
            table.Rows.Add(pt.X, pt.Y);
        }

        return table;
    }
}
