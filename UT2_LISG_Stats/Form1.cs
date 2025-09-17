using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WinForms;
using System.Data;
using System.Globalization;
using System.Text;
using Color = ScottPlot.Color;
using Timer = System.Timers.Timer;

// TESTING IF THIS COMMIT THING WORKS

namespace UT2_LISG_Stats;

public partial class Form1 : Form
{
    int counter = 0;
    int movingAverageWindowSize;
    int slopeWindowSize;

    string db = "test";
    string uid = Environment.GetEnvironmentVariable("TOWER_MYSQL_USER")!;
    string pwd = Environment.GetEnvironmentVariable("TOWER_MYSQL_PASSWORD")!;

    double loessWindowSize;
    bool MouseIsDown = false;
    public string clad;

    List<string> legendItems = new();
    Timer updateData = new Timer(5000);

    Dictionary<string, IPlottable> seriesDict = new Dictionary<string, IPlottable>();

    Coordinates[] DataPoints;
    Coordinates MouseDownCoordinates;
    Coordinates MouseNowCoordinates;
    CoordinateRect MouseSlectionRect => new(MouseDownCoordinates, MouseNowCoordinates);


    DateTime lastDateMS = new DateTime();
    DateTime lastDateMY = new DateTime();

    public ScottPlot.Plottables.Rectangle RectanglePlot;

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
        loessWindowSize = Convert.ToDouble(tbLoess.Text);
    }

    private void tbTower1_Click(object sender, EventArgs e)
    {
        tbTower1.Text = "";
    }
    private void btnExecute_Click(object sender, EventArgs e)
    {
        ClearCharts();
        legendItems?.Clear();

        movingAverageWindowSize = int.Parse(tbMovingAverage.Text);
        slopeWindowSize = int.Parse(tbSlope.Text);
        loessWindowSize = Convert.ToDouble(tbLoess.Text);

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
        string startDate = date.ToString("yyyy-MM-dd HH:mm:ss");
        string startDate2 = date.ToString("yyyy-MM-dd HH:mm:ss");

        DateTime date2;
        DateTime.TryParseExact(poisonDateTime2.Text, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date2);
        string endDate = date2.ToString("yyyy-MM-dd HH:mm:ss");
        string endDate2 = date2.ToString("yyyy-MM-dd HH:mm:ss");

        string query = $"select * from cersacladdata1 where t_stamp >= \"" + startDate + "\" AND t_stamp <= \"" + endDate + "\" AND Diam_Peak_Peak IS NOT NULL AND Cersa_Diam IS NOT NULL  AND Cersa_Xpos_mm IS NOT NULL AND Cersa_Ypos_mm IS NOT NULL order by t_stamp asc";
        string queryChillers = $"select ((LR_Fraction/100)*3) AS LR, ((BF_Fraction/100)*3) AS BF, t_stamp from chiller_position1 where t_stamp >= \"" + startDate + "\" AND t_stamp <= \"" + endDate + "\" AND LR_Fraction IS NOT NULL AND BF_Fraction IS NOT NULL order by t_stamp desc";
        string queryTension = $"select center/100 AS center, power/100 AS power, t_stamp from centvibpowdata1 where t_stamp >= \"" + startDate + "\" AND t_stamp <= \"" + endDate + "\" AND center IS NOT NULL AND power IS NOT NULL order by t_stamp desc";
        string queryLS = $"select LineSpeed, t_stamp from cladhist1 where t_stamp >= \"" + startDate + "\" AND t_stamp <= \"" + endDate + "\" AND LineSpeed IS NOT NULL AND t_stamp IS NOT NULL order by t_stamp desc";
        string queryCladDev = $"select CladDev, t_stamp from cladstatshist1 where t_stamp >= \"" + startDate + "\" AND t_stamp <= \"" + endDate + "\" AND CladDev IS NOT NULL AND t_stamp IS NOT NULL order by t_stamp desc";
        string queryCladUT4 = $"select LineSpeed, Clad, t_stamp from cladhist1 where t_stamp >= \"" + startDate + "\" AND t_stamp <= \"" + endDate + "\" AND LineSpeed IS NOT NULL AND t_stamp IS NOT NULL AND Clad IS NOT NULL order by t_stamp desc";
        string queryClad394 = $"""
                            SELECT (Diam/1000) AS Clad, (Peak2Peak/1000) AS Diam_Peak_Peak, (FibPos/1000) AS Position, t_stamp
                            FROM cersa_details1lisg
                            WHERE t_stamp >= '{startDate}' AND t_stamp <= '{endDate}'
                            AND Diam IS NOT NULL
                            AND t_stamp IS NOT NULL
                            AND Peak2Peak IS NOT NULL
                            AND FibPos IS NOT NULL
                            ORDER BY t_stamp asc
                            """;

        MySqlConnectionManager connection = new MySqlConnectionManager(towerInt, db, uid!, pwd!);
        DataTable dt = new DataTable();
        DataTable dtClad = new DataTable();
        DataTable dtCladExcursion = new DataTable();
        DataTable dtAirlines = new DataTable();
        DataTable dtPressures = new DataTable();
        DataTable dtFurnaceFlows = new DataTable();

        string cladDevQuery, cladExcursionQuery, airlinesQuery, pressuresQuery, furnaceFlowsQuery;
        SetSybaseQueries(towerInt, startDate, endDate, out cladDevQuery, out cladExcursionQuery, out airlinesQuery, out pressuresQuery, out furnaceFlowsQuery);

        try
        {
            var syb = new MsSqlConnectionManager();

            dtClad = syb.Connect(cladDevQuery);
            dtCladExcursion = syb.Connect(cladExcursionQuery);
            dtAirlines = syb.Connect(airlinesQuery);
            dtPressures = syb.Connect(pressuresQuery);
            dtFurnaceFlows = syb.Connect(furnaceFlowsQuery);

            if (towerInt >= 385)
            {
                dt = connection.ExecuteQuery(queryClad394);
            }
            else
            {
                Console.WriteLine();
                dt = connection.ExecuteQuery(query);
            }
        }
        catch (Exception)
        {
            throw;
        }

        DateTime[] xValues = dt.AsEnumerable().Select(x => x.Field<DateTime>("t_stamp")).ToArray();
        DateTime[] cladDevTime = dtClad.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();
        DateTime[] cladExcursionTime = dtCladExcursion.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();
        DateTime[] airlinesTime = dtAirlines.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();
        DateTime[] pressuresTime = dtPressures.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();
        DateTime[] furnaceFlowsTime = dtFurnaceFlows.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();

        List<DateTime[]> msTimesToCheck = new List<DateTime[]>() { cladDevTime, pressuresTime, furnaceFlowsTime};

        Logger.WriteToFileLine("Printing out Last Element of All X Coordinates.");
        foreach (var time in msTimesToCheck)
        {
            Logger.WriteToFileLine($"{nameof(time)}: {time[time.Length - 1]},"); 
        }

        double[] cladDev = dtClad.AsEnumerable().Select(x => x.Field<double>("clad_dev")).ToArray();
        double[] lengthOdometer = dtClad.AsEnumerable().Select(x => (double)x.Field<int>("length_odom") / 1_000_000).ToArray();
        double[] feedPosition = dtClad.AsEnumerable().Select(x => (double)x.Field<int>("feed_pos")).ToArray();
        double[] cladExcursion = dtCladExcursion.AsEnumerable().Select(x => x.Field<double>("datum1")).ToArray();
        double[] bore = dtPressures.AsEnumerable().Select(x => x.Field<double>("BorePressure")).ToArray();
        double[] body = dtPressures.AsEnumerable().Select(x => x.Field<double>("BodyPressure")).ToArray();
        double[] temp = dtPressures.AsEnumerable().Select(x => x.Field<double>("Temperature")).ToArray();
        double[] airlines = Generate.Repeating(airlinesTime.Count(), 0.0);

        double[] boreFlow = dtFurnaceFlows.AsEnumerable().Select(x => x.Field<double>("Ar_Bore")).ToArray();
        double[] sealFlow = dtFurnaceFlows.AsEnumerable().Select(x => x.Field<double>("Ar_Seal")).ToArray();

        formsPlot1.Plot.Add.HorizontalLine(0.6, 2, Colors.Red, LinePattern.Dashed);
        formsPlot1.Plot.Add.HorizontalLine(0.15, 2, Colors.Black, LinePattern.Dashed);

        if (towerInt < 385)
        {
            double[] lr = dt.AsEnumerable().Select(x => (double)x.Field<float>("Cersa_Xpos_mm")).ToArray();
            double[] fb = dt.AsEnumerable().Select(x => (double)x.Field<float>("Cersa_Ypos_mm")).ToArray();
            double[] p2pVals = dt.AsEnumerable().Select(x => (double)x.Field<float>("Diam_Peak_Peak")).ToArray();
            double[] cladVals = dt.AsEnumerable().Select(x => (double)x.Field<float>(clad)).ToArray();

            CreateDataLogger(xValues, cladVals, formsPlot1, "CladDiam", seriesDict, rightAxis: true);
            CreateDataLogger(xValues, p2pVals, formsPlot1, "P2P", seriesDict);
            CreateDataLogger(xValues, lr, formsPlot2, "LR+", seriesDict);
            CreateDataLogger(xValues, fb, formsPlot2, "FB+", seriesDict);

            DataPoints = ConvertToCoordinates(xValues, p2pVals);
        }
        else
        {
            double[] p2pVals = dt.AsEnumerable().Select(x => (double)x.Field<Decimal>("Diam_Peak_Peak")).ToArray();
            double[] cladVals = dt.AsEnumerable().Select(x => (double)x.Field<Decimal>(clad)).ToArray();
            double[] pos = dt.AsEnumerable().Select(x => (double)x.Field<Decimal>("Position")).ToArray();
            CreateScottSignalXYSecondary(xValues, cladVals, formsPlot1, "CladDiam", seriesDict);
            //CreateSignalXY(xValues, cladVals, formsPlot1, "CladDiam", seriesDict, rightAxis: true);
            CreateScottSignalXY(xValues, p2pVals, formsPlot1, "P2P", seriesDict);
            //CreateSignalXY(xValues, p2pVals, formsPlot1, "P2P", seriesDict);
            CreateScottSignalXY(xValues, pos, formsPlot2, "Position", seriesDict);
            //CreateSignalXY(xValues, pos, formsPlot2, "Position", seriesDict);

            DataPoints = ConvertToCoordinates(xValues, p2pVals);
        }
        Color colorLime = ScottPlot.Color.FromSDColor(System.Drawing.Color.Lime);
        Color colorRed = ScottPlot.Color.FromSDColor(System.Drawing.Color.Red);
        Color colorLightGray = ScottPlot.Color.FromSDColor(System.Drawing.Color.LightGray);
        Color colorCrimson = ScottPlot.Color.FromSDColor(System.Drawing.Color.Crimson);
        Color colorMediumPurple = ScottPlot.Color.FromSDColor(System.Drawing.Color.MediumPurple);
        Color colorGreen = ScottPlot.Color.FromSDColor(System.Drawing.Color.Green);
        Color colorYellow = ScottPlot.Color.FromSDColor(System.Drawing.Color.Yellow);
        Color colorMediumSlateBlue = ScottPlot.Color.FromSDColor(System.Drawing.Color.MediumSlateBlue);

        CreateDataLogger(cladDevTime, cladDev, formsPlot1, "CladDev", seriesDict, color: colorLime);
        CreateScatter(cladExcursionTime, cladExcursion, formsPlot1, "CladExcursion", seriesDict, color: colorRed, noLine: true, rightAxis: true);
        CreateScatter(airlinesTime, airlines, formsPlot1, "Airlines", seriesDict, noLine: true, rightAxis: true);
        CreateDataLogger(pressuresTime, body, formsPlot3, "Body(Pa)", seriesDict, color: colorLightGray);
        CreateDataLogger(pressuresTime, temp, formsPlot3, "Temp", seriesDict, color: colorCrimson, rightAxis: true);
        CreateDataLogger(pressuresTime, bore, formsPlot3, "Bore(Pa)", seriesDict, color: colorMediumPurple);

        CreateDataLogger(furnaceFlowsTime, boreFlow, formsPlot3, "BoreFlow", seriesDict, color: colorGreen);
        CreateDataLogger(furnaceFlowsTime, sealFlow, formsPlot3, "SealFlow", seriesDict, color: colorYellow);

        CreateDataLogger(cladDevTime, lengthOdometer, formsPlot1, "len_odom (Mm)", seriesDict, color: colorMediumSlateBlue);
        CreateDataLogger(cladDevTime, feedPosition, formsPlot2, "feed_pos", seriesDict, color: colorMediumSlateBlue, rightAxis: true);

        var plot1X = formsPlot1.Plot.Axes.Bottom;
        var plot2X = formsPlot2.Plot.Axes.Bottom;
        var plot3X = formsPlot3.Plot.Axes.Bottom;

        formsPlot1.Plot.Axes.Link(plot1X, plot2X, formsPlot2.Plot);
        formsPlot1.Plot.Axes.Link(plot1X, plot3X, formsPlot3.Plot);
        formsPlot2.Plot.Axes.Link(plot2X, plot1X, formsPlot1.Plot);
        formsPlot2.Plot.Axes.Link(plot2X, plot3X, formsPlot3.Plot);
        formsPlot3.Plot.Axes.Link(plot3X, plot1X, formsPlot1.Plot);
        formsPlot3.Plot.Axes.Link(plot3X, plot2X, formsPlot2.Plot);

        formsPlot1.Plot.Legend.FontSize = 8;
        formsPlot2.Plot.Legend.FontSize = 8;
        formsPlot3.Plot.Legend.FontSize = 8;

        formsPlot1.Refresh();
        formsPlot2.Refresh();
        formsPlot3.Refresh();

        var pointsAnnotation = formsPlot1.Plot.Add.Annotation("", Alignment.UpperLeft);
        var pointsAnnotation2 = formsPlot2.Plot.Add.Annotation("", Alignment.UpperLeft);
        var pointsAnnotation3 = formsPlot3.Plot.Add.Annotation("", Alignment.UpperLeft);

        HoverManager hoverManager = new HoverManager(formsPlot1);
        hoverManager.PointHovered += (plottable, index, x, y) =>
        {
            pointsAnnotation.Text = $"X = {DateTime.FromOADate(x).ToString()}     Y={y:0.##}";
        };

        hoverManager.NoPointHovered += () =>
        {
            pointsAnnotation.Text = "No point hovered";
        };

        HoverManager hoverManager2 = new HoverManager(formsPlot2);
        hoverManager2.PointHovered += (plottable, index, x, y) =>
        {
            pointsAnnotation2.Text = $"X = {DateTime.FromOADate(x).ToString()}     Y={y:0.##}";
        };

        hoverManager2.NoPointHovered += () =>
        {
            pointsAnnotation2.Text = "No point hovered";
        };

        HoverManager hoverManager3 = new HoverManager(formsPlot3);
        hoverManager3.PointHovered += (plottable, index, x, y) =>
        {
            pointsAnnotation3.Text = $"X = {DateTime.FromOADate(x).ToString()}     Y = {y:0.##}";
        };

        hoverManager3.NoPointHovered += () =>
        {
            pointsAnnotation3.Text = "No point hovered";
        };


        if (xValues.Length > 0)
        {
            lastDateMY = xValues[xValues.Length - 1];
            Logger.WriteToFileLine($"Initialized lastDateMY: {lastDateMY.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        else
        {
            MessageBox.Show("MySQL data is empty!");
        }

        if (pressuresTime.Length > 0)
        {
            lastDateMS= pressuresTime[pressuresTime.Length - 1];
            Logger.WriteToFileLine($"Initialized lastDateMS: {lastDateMS.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        else
        {
            MessageBox.Show("Furance Flows data is empty!");
        }

        formsPlot1.Plot.Legend.Alignment = Alignment.LowerLeft;
        formsPlot2.Plot.Legend.Alignment = Alignment.LowerLeft;
        formsPlot3.Plot.Legend.Alignment = Alignment.LowerLeft;
    }

    private static void SetSybaseQueries(int towerInt, string startDate, string endDate, out string cladDevQuery, out string cladExcursionQuery, out string airlinesQuery, out string pressuresQuery, out string furnaceFlowsQuery)
    {
        cladDevQuery = Queries.SybaseCladDeviation(towerInt.ToString(), startDate, endDate);
        cladExcursionQuery = Queries.SybaseCladExcursion(towerInt.ToString(), startDate, endDate);
        airlinesQuery = Queries.SybaseAirlines(towerInt.ToString(), startDate, endDate);
        pressuresQuery = Queries.SybaseFurnacePressures(towerInt.ToString(), startDate, endDate);
        furnaceFlowsQuery = Queries.SybaseFurnaceFlows(towerInt.ToString(), startDate, endDate);
    }

    public void ClearCharts()
    {
        formsPlot1.Reset();
        formsPlot2.Reset();
        formsPlot3.Reset();
        formsPlot1.Refresh();
        formsPlot2.Refresh();
        formsPlot3.Refresh();
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
    public static void CreateScottSignalXYSecondary(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, System.Drawing.Color color)
    {
        var scatter = formsPlot.Plot.Add.SignalXY(xs, ydata);
        scatter.Axes.YAxis = formsPlot.Plot.Axes.Right;
        scatter.LegendText = legendName;
        scatter.LineColor = Color.FromColor(color);
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
    public static void CreateScottScatterNoLineSecondary(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, Dictionary<string, IPlottable> dictionary)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
        scatter.LineWidth = 0;
        scatter.LegendText = legendName;
        scatter.Axes.YAxis = formsPlot.Plot.Axes.Right;
        formsPlot.Plot.Axes.DateTimeTicksBottom();
        dictionary[legendName] = scatter;
        formsPlot.Refresh();
    }
    public static void CreateScottScatterNoLineSecondary(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, Dictionary<string, IPlottable> dictionary, System.Drawing.Color color)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
        scatter.LineWidth = 0;
        scatter.LegendText = legendName;
        scatter.Color = Color.FromColor(color);
        scatter.Axes.YAxis = formsPlot.Plot.Axes.Right;
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
        loessWindowSize = Convert.ToDouble(tbLoess.Text);

        // clear old markers
        formsPlot1.Plot.Remove<ScottPlot.Plottables.Marker>();

        if (counter > 0)
        {
            formsPlot1.Plot.Remove(seriesDict["Moving Average"]);
            formsPlot1.Plot.Remove(seriesDict["Slope M.Avg"]);
            formsPlot1.Plot.Remove(seriesDict["Loess Fit"]);
            formsPlot1.Plot.Remove(seriesDict["Slope Loess"]);
            seriesDict.Remove("Moving Average");
            seriesDict.Remove("Slope M.Avg");
            seriesDict.Remove("Loess Fit");
            seriesDict.Remove("Slope Loess");
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

        DateTime[] xValues = myTable.AsEnumerable().Select(x => DateTime.FromOADate(x.Field<double>("X"))).ToArray();
        double[] yMovingAverage = myTable.AsEnumerable().Select(x => (double)x.Field<float>("Y_MovingAvg")).ToArray();
        double[] ySlopeValues = myTable.AsEnumerable().Select(x => (double)x.Field<float>("Y_Slope")).ToArray();

        double[] loessXValues = xValues.AsEnumerable().Select(x => x.ToOADate()).ToArray();
        double[] loessYValues = Loess.OptimizedLoess(loessXValues, yMovingAverage, loessWindowSize);
        double[] loessSlope = TankStateAnalyzer.CalculateSlope(loessYValues, xValues, slopeWindowSize);

        CreateScottSignalXY(xValues, loessYValues, formsPlot1, "Loess Fit", System.Drawing.Color.LightSeaGreen, seriesDict);
        CreateScottSignalXY(xValues, yMovingAverage, formsPlot1, "Moving Average", System.Drawing.Color.BlueViolet, seriesDict);
        CreateScottSignalXY(xValues, ySlopeValues, formsPlot1, "Slope M.Avg", System.Drawing.Color.Magenta, seriesDict);
        CreateScottSignalXY(xValues, loessSlope, formsPlot1, "Slope Loess", System.Drawing.Color.PowderBlue, seriesDict);

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
    private void formsPlot1_ClientSizeChanged(object sender, EventArgs e)
    {
        formsPlot2.Width = formsPlot1.Width - 13;
        formsPlot2.Height = formsPlot1.Height / 2;

        formsPlot3.Width = formsPlot1.Width - 13;
        formsPlot3.Height = formsPlot1.Height / 2;
    }
    public static IPlottable CreateScatter(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string legendName, Dictionary<string, IPlottable>? dict = null,
        Color? color = null, bool noLine = false, bool rightAxis = false)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ys);
        scatter.LegendText = legendName;

        if (color.HasValue)
            scatter.LineColor = color.Value;

        if (noLine)
            scatter.LineWidth = 0;

        if (rightAxis)
            scatter.Axes.YAxis = formsPlot.Plot.Axes.Right;

        dict[legendName] = scatter;

        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
        return scatter;
    }

    public static IPlottable CreateSignalXY(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string legendName, Dictionary<string, IPlottable>? dict = null,
        Color? color = null, bool rightAxis = false)
    {
        var sig = formsPlot.Plot.Add.SignalXY(xs, ys);
        sig.LegendText = legendName;

        if (color.HasValue)
            sig.LineColor = color.Value;

        if (rightAxis)
            sig.Axes.YAxis = formsPlot.Plot.Axes.Right;

        dict[legendName] = sig;

        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
        return sig;
    }

    public static IPlottable CreateDataLogger(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string legendName, Dictionary<string, IPlottable>? dict = null,
        Color? color = null, bool rightAxis = false)
    {
        var logger = formsPlot.Plot.Add.DataLogger();
        logger.LegendText = legendName;
        logger.ManageAxisLimits = false;

        if (color.HasValue)
            logger.Color = color.Value;

        if (rightAxis)
            logger.Axes.YAxis = formsPlot.Plot.Axes.Right;

        // DataLogger only takes double[], so convert DateTime[] to OADate
        double[] xOADates = Array.ConvertAll(xs, dt => dt.ToOADate());
        logger.Add(xOADates, ys);

        dict[legendName] = logger;

        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
        return logger;
    }

    private void cbDataUpdate_CheckedChanged(object sender, EventArgs e)
    {
        this.BeginInvoke((MethodInvoker)delegate
        {
            if (cbDataUpdate.Checked)
            {
                updateData.Enabled = true;
                updateData.AutoReset = true;
                updateData.Elapsed += UpdateData_Elapsed;
                updateData.Start();
            }
            else
            {
                updateData.Stop();
                updateData.Enabled = false;
            }
        });
    }

    private void UpdateData_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        int.TryParse(tbTower1.Text, out int towerInt);

        if (towerInt >= 385)
        {
            clad = "Clad";
        }
        else
        {
            clad = "Cersa_Diam";
        }

        string endDateMY = lastDateMY.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
        string endDateMS = lastDateMS.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
        string startDateMY = lastDateMY.ToString("yyyy-MM-dd HH:mm:ss");
        string startDateMS = lastDateMS.ToString("yyyy-MM-dd HH:mm:ss");

        Logger.WriteToFile($"lastDateMS: {startDateMS}\t");
        Logger.WriteToFileLine($"lastDateMY: {startDateMY}");

        string query = $"select * from cersacladdata1 where t_stamp > \"" + startDateMY + "\" AND t_stamp <= \"" + endDateMY + "\" AND Diam_Peak_Peak IS NOT NULL AND Cersa_Diam IS NOT NULL  AND Cersa_Xpos_mm IS NOT NULL AND Cersa_Ypos_mm IS NOT NULL order by t_stamp asc";
        string queryChillers = $"select ((LR_Fraction/100)*3) AS LR, ((BF_Fraction/100)*3) AS BF, t_stamp from chiller_position1 where t_stamp > \"" + startDateMY + "\" AND t_stamp <= \"" + endDateMY + "\" AND LR_Fraction IS NOT NULL AND BF_Fraction IS NOT NULL order by t_stamp desc";
        string queryTension = $"select center/100 AS center, power/100 AS power, t_stamp from centvibpowdata1 where t_stamp > \"" + startDateMY + "\" AND t_stamp <= \"" + endDateMY + "\" AND center IS NOT NULL AND power IS NOT NULL order by t_stamp desc";
        string queryLS = $"select LineSpeed, t_stamp from cladhist1 where t_stamp > \"" + startDateMY + "\" AND t_stamp <= \"" + endDateMY + "\" AND LineSpeed IS NOT NULL AND t_stamp IS NOT NULL order by t_stamp desc";
        string queryCladDev = $"select CladDev, t_stamp from cladstatshist1 where t_stamp > \"" + startDateMY + "\" AND t_stamp <= \"" + endDateMY + "\" AND CladDev IS NOT NULL AND t_stamp IS NOT NULL order by t_stamp desc";
        string queryCladUT4 = $"select LineSpeed, Clad, t_stamp from cladhist1 where t_stamp > \"" + startDateMY + "\" AND t_stamp <= \"" + endDateMY + "\" AND LineSpeed IS NOT NULL AND t_stamp IS NOT NULL AND Clad IS NOT NULL order by t_stamp desc";
        string queryClad394 = $"""
                            SELECT (Diam/1000) AS Clad, (Peak2Peak/1000) AS Diam_Peak_Peak, (FibPos/1000) AS Position, t_stamp
                            FROM cersa_details1lisg
                            WHERE t_stamp > '{startDateMY}' AND t_stamp <= '{endDateMY}'
                            AND Diam IS NOT NULL
                            AND t_stamp IS NOT NULL
                            AND Peak2Peak IS NOT NULL
                            AND FibPos IS NOT NULL
                            ORDER BY t_stamp asc
                            """;

        MySqlConnectionManager connection = new MySqlConnectionManager(towerInt, db, uid!, pwd!);
        DataTable dt = new DataTable();
        DataTable dtClad = new DataTable();
        DataTable dtCladExcursion = new DataTable();
        DataTable dtAirlines = new DataTable();
        DataTable dtPressures = new DataTable();
        DataTable dtFurnaceFlows = new DataTable();


        string cladDevQuery, cladExcursionQuery, airlinesQuery, pressuresQuery, furnaceFlowsQuery;
        SetSybaseQueries(towerInt, startDateMS, endDateMS, out cladDevQuery, out cladExcursionQuery, out airlinesQuery, out pressuresQuery, out furnaceFlowsQuery);

        try
        {
            var syb = new MsSqlConnectionManager();
            
            dtClad = syb.Connect(cladDevQuery);
            //dtCladExcursion = syb.SqlConnect(cladExcursionQuery);
            //dtAirlines = syb.SqlConnect(airlinesQuery);
            dtPressures = syb.Connect(pressuresQuery);
            dtFurnaceFlows = syb.Connect(furnaceFlowsQuery);

            if (towerInt >= 385)
            {
                dt = connection.ExecuteQuery(queryClad394);
            }
            else
            {
                Console.WriteLine();
                dt = connection.ExecuteQuery(query);
            }
        }
        catch (Exception)
        {
            throw;
        }

        if (dt.Rows.Count > 0)
        {
            DateTime[] xValues = dt.AsEnumerable().Select(x => x.Field<DateTime>("t_stamp")).ToArray();

            if (towerInt < 385)
            {
                double[] lr = dt.AsEnumerable().Select(x => (double)x.Field<float>("Cersa_Xpos_mm")).ToArray();
                double[] fb = dt.AsEnumerable().Select(x => (double)x.Field<float>("Cersa_Ypos_mm")).ToArray();
                double[] p2pVals = dt.AsEnumerable().Select(x => (double)x.Field<float>("Diam_Peak_Peak")).ToArray();
                double[] cladVals = dt.AsEnumerable().Select(x => (double)x.Field<float>(clad)).ToArray();

                Coordinates[] pointsLR = ConvertToCoordinates(xValues, lr);
                Coordinates[] pointsFB = ConvertToCoordinates(xValues, fb);
                Coordinates[] pointsP2P = ConvertToCoordinates(xValues, p2pVals);
                Coordinates[] pointsClad = ConvertToCoordinates(xValues, cladVals);

                var loggerLR = (ScottPlot.Plottables.DataLogger)seriesDict["LR+"];
                var loggerFB = (ScottPlot.Plottables.DataLogger)seriesDict["FB+"];
                var loggerP2P = (ScottPlot.Plottables.DataLogger)seriesDict["P2P"];
                var loggerClad = (ScottPlot.Plottables.DataLogger)seriesDict["CladDiam"];

                try
                {
                    loggerLR.Add(pointsLR);
                    loggerFB.Add(pointsFB);
                    loggerP2P.Add(pointsP2P);
                    loggerClad.Add(pointsClad);
                }
                catch (ArgumentException ex)
                {
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Source + "\n\n" + ex.InnerException + "\n\n" + ex.StackTrace + "\n\n" + ex.Data + "\n\n" + ex.GetType().ToString());
                }
            }
            else
            {
                // double[] p2pVals = dt.AsEnumerable().Select(x => (double)x.Field<Decimal>("Diam_Peak_Peak")).ToArray();
                // double[] cladVals = dt.AsEnumerable().Select(x => (double)x.Field<Decimal>(clad)).ToArray();
                // double[] pos = dt.AsEnumerable().Select(x => (double)x.Field<Decimal>("Position")).ToArray();
                // CreateScottSignalXYSecondary(xValues, cladVals, formsPlot1, "CladDiam", seriesDict);
                // CreateScottSignalXY(xValues, p2pVals, formsPlot1, "P2P", seriesDict);
                // CreateScottSignalXY(xValues, pos, formsPlot2, "Position", seriesDict);
                // DataPoints = ConvertToCoordinates(xValues, p2pVals);
            }
            lastDateMY = xValues[xValues.Length - 1];
            // Logger.WriteToFileLine($"Updating New Value lastDateMY: {lastDateMY.ToString("yyyy-MM-dd HH:mm:ss")}; dt rows: {dt.Rows.Count}");
            // Logger.WriteToFileLine("dt rows below:");
            // Logger.WriteDataTable(dt, dt.Rows.Count);
        }

        if (dtPressures.Rows.Count > 0)
        {
            Logger.WriteToFileLine($"\ndtPressures rows: {dtPressures.Rows.Count}\n");
            DateTime[] cladDevTime = dtClad.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();
            //DateTime[] cladExcursionTime = dtCladExcursion.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();
            //DateTime[] airlinesTime = dtAirlines.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();
            DateTime[] pressuresTime = dtPressures.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();
            DateTime[] furnaceFlowsTime = dtFurnaceFlows.AsEnumerable().Select(x => x.Field<DateTime>("event_ts")).ToArray();

            var msTimesToCheck = new Dictionary<string, DateTime[]>
            {
                { nameof(cladDevTime), cladDevTime },
                { nameof(pressuresTime), pressuresTime },
                { nameof(furnaceFlowsTime), furnaceFlowsTime }
            };

            Logger.WriteToFileLine("Printing out 1st Element of All X Coordinates.");
            foreach (var time in msTimesToCheck)
            {
                var name = time.Key;
                var array = time.Value;

                try
                {
                    Logger.WriteToFileLine($"{name}: {array[0]},");
                }
                catch (IndexOutOfRangeException)
                {
                    Logger.WriteToFileLine($"{name} is empty. Skipping...");
                }
                catch (Exception ex)
                {
                    Logger.WriteToFileLine($"Exception when logging: {name} -> {ex}");
                }
            }

            double[] cladDev = dtClad.AsEnumerable().Select(x => x.Field<double>("clad_dev")).ToArray();
            double[] lengthOdometer = dtClad.AsEnumerable().Select(x => (double)x.Field<int>("length_odom") / 1_000_000).ToArray();
            double[] feedPosition = dtClad.AsEnumerable().Select(x => (double)x.Field<int>("feed_pos")).ToArray();
            //double[] cladExcursion = dtCladExcursion.AsEnumerable().Select(x => x.Field<double>("datum1")).ToArray();
            double[] bore = dtPressures.AsEnumerable().Select(x => x.Field<double>("BorePressure")).ToArray();
            double[] body = dtPressures.AsEnumerable().Select(x => x.Field<double>("BodyPressure")).ToArray();
            double[] temp = dtPressures.AsEnumerable().Select(x => x.Field<double>("Temperature")).ToArray();
            //double[] airlines = Generate.Repeating(airlinesTime.Count(), 0.0);

            double[] boreFlow = dtFurnaceFlows.AsEnumerable().Select(x => x.Field<double>("Ar_Bore")).ToArray();
            double[] sealFlow = dtFurnaceFlows.AsEnumerable().Select(x => x.Field<double>("Ar_Seal")).ToArray();


            Coordinates[] pointsCladDev = ConvertToCoordinates(cladDevTime, cladDev);
            //Coordinates[] pointsCladExcursions = ConvertToCoordinates(cladExcursionTime, cladExcursion);
            //Coordinates[] pointsAirlines = ConvertToCoordinates(airlinesTime, airlines);

            Coordinates[] pointsBody = ConvertToCoordinates(pressuresTime, body);
            Coordinates[] pointsTemp = ConvertToCoordinates(pressuresTime, temp);
            Coordinates[] pointsBore = ConvertToCoordinates(pressuresTime, bore);

            Coordinates[] pointsBoreFlow = ConvertToCoordinates(furnaceFlowsTime, boreFlow);
            Coordinates[] pointsSealFlow = ConvertToCoordinates(furnaceFlowsTime, sealFlow);

            Coordinates[] pointsLengthOdometer = ConvertToCoordinates(cladDevTime, lengthOdometer);
            Coordinates[] pointsFeedPosition = ConvertToCoordinates(cladDevTime, feedPosition);

            var loggerCladDev = (ScottPlot.Plottables.DataLogger)seriesDict["CladDev"];
            //var loggerCladExcursion = (ScottPlot.Plottables.DataLogger)seriesDict["CladExcursion"];
            //var loggerAirlines = (ScottPlot.Plottables.DataLogger)seriesDict["Airlines"];
            var loggerBody = (ScottPlot.Plottables.DataLogger)seriesDict["Body(Pa)"];
            var loggerTemp = (ScottPlot.Plottables.DataLogger)seriesDict["Temp"];
            var loggerBore = (ScottPlot.Plottables.DataLogger)seriesDict["Bore(Pa)"];

            var loggerBoreFlow = (ScottPlot.Plottables.DataLogger)seriesDict["BoreFlow"];
            var loggerSealFlow = (ScottPlot.Plottables.DataLogger)seriesDict["SealFlow"];

            var loggerLengthOdometer = (ScottPlot.Plottables.DataLogger)seriesDict["len_odom (Mm)"];
            var loggerFeedPosition = (ScottPlot.Plottables.DataLogger)seriesDict["feed_pos"];

            Dictionary<ScottPlot.Plottables.DataLogger, Coordinates[]> logger = new Dictionary<ScottPlot.Plottables.DataLogger, Coordinates[]>();
            logger.Add(loggerCladDev, pointsCladDev);
            logger.Add(loggerBody, pointsBody);
            logger.Add(loggerTemp, pointsTemp);
            logger.Add(loggerBore, pointsBore);
            logger.Add(loggerBoreFlow, pointsBoreFlow);
            logger.Add(loggerSealFlow, pointsSealFlow);
            logger.Add(loggerLengthOdometer, pointsLengthOdometer);
            logger.Add(loggerFeedPosition, pointsFeedPosition);

            foreach (var log in logger)
            {
                var currentLogger = log.Key;
                try
                {
                    currentLogger.Add(log.Value);
                }
                catch(ArgumentException)
                {
                }
                catch (Exception ex)
                {
                    Logger.WriteToFileLine($"Exception when appending to logger: {ex}");
                    Logger.WriteToFileLine($"\nlastDateMS Value at exception: {lastDateMS.ToString("yyyy-MM-dd HH:mm:ss")}");
                    Logger.WriteToFileLine("dtPressures:");
                    Logger.WriteDataTable(dtPressures, dtPressures.Rows.Count);
                } 
            }

            lastDateMS = pressuresTime[pressuresTime.Length - 1];
            Logger.WriteToFileLine($"\nUpdating New Value lastDateMS: {lastDateMS.ToString("yyyy-MM-dd HH:mm:ss")}");
            Logger.WriteToFileLine("dtPressures:");
            Logger.WriteDataTable(dtPressures, dtPressures.Rows.Count);
            Logger.WriteToFileLine("");

            // Adding this so can try/catch and skip
            Logger.WriteToFileLine("Printing out Last Element of All X Coordinates.");
            foreach (var time in msTimesToCheck)
            {
                var name = time.Key;
                var array = time.Value;

                try
                {
                    Logger.WriteToFileLine($"{name}: {array[array.Length - 1]},");
                }
                catch (IndexOutOfRangeException)
                {
                    Logger.WriteToFileLine($"{name} is empty. Skipping...");
                }
                catch (Exception ex)
                {
                    Logger.WriteToFileLine($"Exception when logging: {name} -> {ex}");
                }
            }
        }

        formsPlot1.Invoke((MethodInvoker)(() =>
        {
            formsPlot1.Refresh();
        }));

        formsPlot2.Invoke((MethodInvoker)(() =>
        {
            formsPlot2.Refresh();
        }));

        formsPlot3.Invoke((MethodInvoker)(() =>
        {
            formsPlot3.Refresh();
        }));

    }
}