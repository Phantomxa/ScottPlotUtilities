using ScottPlot.WinForms;
using ScottPlot.Plottables;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = ScottPlot.Color;
using System.Data;
using System.Globalization;
using ScottPlot.Plottables;
using ScottPlot.Statistics;
using ScottPlot.TickGenerators;

namespace ScottPlotUtilities;

public static class ScottPlotGraph
{
    #region Scatter / SignalXY / DataLogger

    public static IPlottable CreateScatter(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string? legendName = null,
        Dictionary<string, IPlottable>? dict = null,
        Color? color = null,
        bool noLine = false,
        bool rightAxis = false)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ys);
        if (legendName is not null) scatter.LegendText = legendName;
        if (color.HasValue) scatter.LineColor = color.Value;
        if (noLine) scatter.LineWidth = 0;
        if (rightAxis) scatter.Axes.YAxis = formsPlot.Plot.Axes.Right;
        dict[legendName] = scatter;

        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Plot.Axes.AutoScale();
        formsPlot.Refresh();
        return scatter;
    }
    public static Scatter CreateScatter(double[] xs, double[] ys, FormsPlot formsPlot,
        string? legendName = null,
        Dictionary<string, IPlottable>? dict = null,
        Color? color = null,
        bool noLine = false,
        bool rightAxis = false)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ys);
        if (legendName is not null) scatter.LegendText = legendName;
        if (color.HasValue) scatter.LineColor = color.Value;
        if (noLine) scatter.LineWidth = 0;
        if (rightAxis) scatter.Axes.YAxis = formsPlot.Plot.Axes.Right;
        if (dict is not null) dict[legendName] = scatter;

        formsPlot.Plot.Axes.AutoScale();
        formsPlot.Refresh();
        return scatter;
    }
    public static IPlottable CreateSignalXY(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string legendName,
        Dictionary<string, IPlottable>? dict = null,
        Color? color = null,
        bool rightAxis = false)
    {
        var sig = formsPlot.Plot.Add.SignalXY(xs, ys);
        sig.LegendText = legendName;
        if (color.HasValue) sig.LineColor = color.Value;
        if (rightAxis) sig.Axes.YAxis = formsPlot.Plot.Axes.Right;
        dict[legendName] = sig;

        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
        return sig;
    }

    public static IPlottable CreateDataLogger(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string legendName,
        Dictionary<string, IPlottable>? dict = null,
        Color? color = null,
        bool rightAxis = false)
    {
        var logger = formsPlot.Plot.Add.DataLogger();
        logger.LegendText = legendName;
        logger.ManageAxisLimits = false;
        if (color.HasValue) logger.Color = color.Value;
        if (rightAxis) logger.Axes.YAxis = formsPlot.Plot.Axes.Right;

        double[] xOADates = Array.ConvertAll(xs, dt => dt.ToOADate());
        logger.Add(xOADates, ys);
        dict[legendName] = logger;

        formsPlot.Plot.Axes.DateTimeTicksBottom();
        formsPlot.Refresh();
        return logger;
    }

    public static IPlottable CreateScatterNoLine(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string legendName,
        Dictionary<string, IPlottable>? dict = null,
        Color? color = null,
        bool rightAxis = false)
    {
        return CreateScatter(xs, ys, formsPlot, legendName, dict, color, noLine: true, rightAxis);
    }

    public static IPlottable CreateSignalXYSecondary(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string legendName,
        Dictionary<string, IPlottable>? dict = null,
        Color? color = null)
    {
        return CreateSignalXY(xs, ys, formsPlot, legendName, dict, color, rightAxis: true);
    }

    public static IPlottable CreateScatterSecondary(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string legendName,
        Dictionary<string, IPlottable>? dict = null,
        Color? color = null,
        bool noLine = false)
    {
        return CreateScatter(xs, ys, formsPlot, legendName, dict, color, noLine, rightAxis: true);
    }

    #endregion

    #region Bar Charts

    public static void FillByGroupCountBar(Dictionary<string?, int> values, FormsPlot formsPlot)
    {
        int counter = 1;
        var ticks = new Tick[values.Count];

        foreach (var item in values)
        {
            var bar = formsPlot.Plot.Add.Bar(counter, item.Value);
            bar.Horizontal = true;
            ticks[counter - 1] = new Tick(counter, item.Key);
            counter++;
        }

        formsPlot.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
        formsPlot.Plot.Axes.Left.MajorTickStyle.Length = 0;
        formsPlot.Plot.Axes.Margins(left: 0);
        formsPlot.Plot.HideGrid();
        formsPlot.Plot.Axes.AutoScale();
        formsPlot.Refresh();
    }

    public static void FillByGroupCountPareto(Dictionary<string?, int> values, FormsPlot formsPlot)
    {
        var ordered = values.OrderBy(x => x.Value).ToArray();
        int counter = 1;
        var ticks = new Tick[ordered.Length];

        foreach (var item in ordered)
        {
            var bar = formsPlot.Plot.Add.Bar(counter, item.Value);
            bar.Horizontal = true;
            ticks[counter - 1] = new Tick(counter, item.Key);
            counter++;
        }

        formsPlot.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
        formsPlot.Plot.Axes.Left.MajorTickStyle.Length = 0;
        formsPlot.Plot.Axes.Margins(left: 0);
        formsPlot.Plot.HideGrid();
        formsPlot.Plot.Axes.AutoScale();
        formsPlot.Refresh();
    }

    #endregion

    #region BoxPlots
    public enum BoxGrouping
    {
        Yyyymmdd,   // group by date.ToString("yyyyMMdd")
        DateOnly,   // group by date.Date (label as yyyy-MM-dd)
        Category    // group by a string column/category
    }

    /// <summary>
    /// Create a boxplot (one box per group) from a DataTable.
    /// groupColumn: column name that contains a DateTime (for Yyyymmdd/DateOnly) or string (for Category).
    /// valueColumn: column name that contains numeric values (double, float, int, etc.).
    /// Returns the IPlottable produced (the Boxes group).
    /// </summary>
    public static IPlottable CreateBoxPlotFromTable(
        DataTable table,
        string groupColumn,
        string valueColumn,
        BoxGrouping grouping,
        FormsPlot formsPlot,
        string legendName = "",
        Dictionary<string, IPlottable>? dict = null,
        System.Drawing.Color? color = null,
        bool rightAxis = false,
        string? title = null,
        System.Drawing.Color? lineColor = null)
    {
        if (table == null) throw new ArgumentNullException(nameof(table));
        if (formsPlot == null) throw new ArgumentNullException(nameof(formsPlot));
        if (!table.Columns.Contains(groupColumn)) throw new ArgumentException($"Table has no column '{groupColumn}'", nameof(groupColumn));
        if (!table.Columns.Contains(valueColumn)) throw new ArgumentException($"Table has no column '{valueColumn}'", nameof(valueColumn));

        // Build groups: key -> List<double>
        var groups = new List<(string Label, double Position, List<double> Values)>();

        if (grouping == BoxGrouping.Category)
        {
            var grouped = table.AsEnumerable()
                .GroupBy(r => r.Field<string?>(groupColumn) ?? string.Empty)
                .OrderBy(g => g.Key);

            int i = 1;
            foreach (var g in grouped)
            {
                var values = g.Select(r => Convert.ToDouble(r[valueColumn])).Where(d => !double.IsNaN(d) && !double.IsInfinity(d)).ToList();
                if (values.Count == 0) continue;
                groups.Add((Label: g.Key, Position: i, Values: values));
                i++;
            }
        }
        else // Yyyymmdd or DateOnly
        {
            // ensure the group column is DateTime
            var rowsWithDates = table.AsEnumerable()
                .Select(r =>
                {
                    object? obj = r[groupColumn];
                    if (obj is DateTime dt) return (Ok: true, Dt: dt, Row: r);
                    // try convert
                    if (DateTime.TryParse(Convert.ToString(obj), out DateTime parsed))
                        return (Ok: true, Dt: parsed, Row: r);
                    return (Ok: false, Dt: DateTime.MinValue, Row: r);
                })
                .Where(x => x.Ok)
                .ToArray();

            if (rowsWithDates.Length == 0)
                throw new ArgumentException($"Group column '{groupColumn}' has no valid DateTime values.");

            IEnumerable<IGrouping<string, (DateTime Dt, DataRow Row)>> grouped;

            if (grouping == BoxGrouping.Yyyymmdd)
            {
                grouped = (IEnumerable<IGrouping<string, (DateTime Dt, DataRow Row)>>)rowsWithDates
                    .GroupBy(x => x.Dt.ToString("yyyyMMdd", CultureInfo.InvariantCulture))
                    .Select(g => g);
            }
            else // DateOnly
            {
                grouped = (IEnumerable<IGrouping<string, (DateTime Dt, DataRow Row)>>)rowsWithDates
                    .GroupBy(x => x.Dt.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                    .Select(g => g);
            }

            foreach (var g in grouped.OrderBy(g => g.Key))
            {
                var values = g.Select(x => Convert.ToDouble(x.Row[valueColumn])).Where(d => !double.IsNaN(d) && !double.IsInfinity(d)).ToList();
                if (values.Count == 0) continue;

                // determine position (OADate) from the group's representative DateTime (first result)
                DateTime repDt = g.First().Dt;
                double pos = (grouping == BoxGrouping.Yyyymmdd) ? DateTime.ParseExact(g.Key, "yyyyMMdd", CultureInfo.InvariantCulture).ToOADate()
                                    : DateTime.ParseExact(g.Key, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToOADate();

                // user-visible label (for category-like axis if desired)
                string label = (grouping == BoxGrouping.Yyyymmdd) ? DateTime.ParseExact(g.Key, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                                  : g.Key;

                groups.Add((Label: label, Position: pos, Values: values));
            }
        }

        if (groups.Count == 0)
            throw new InvalidOperationException("No groups with numeric values were found.");

        // Create box objects
        var boxes = new List<ScottPlot.Box>();
        int idx = 0;
        foreach (var grp in groups)
        {
            idx++;
            var stats = ComputeBoxStats(grp.Values);
            var box = new ScottPlot.Box()
            {
                Position = grp.Position,
                BoxMin = stats.Q1,
                BoxMax = stats.Q3,
                BoxMiddle = stats.Median,
                WhiskerMin = stats.WhiskerMin,
                WhiskerMax = stats.WhiskerMax,
            };

            if (color.HasValue)
                box.FillColor = ScottPlot.Color.FromColor(color.Value);
            if (lineColor.HasValue)
                box.LineColor = ScottPlot.Color.FromColor(lineColor.Value);
            boxes.Add(box);
        }

        // Add all boxes as a single Boxes plottable (gives a single legend entry)
        var boxesPlottable = formsPlot.Plot.Add.Boxes(boxes);
        if (!string.IsNullOrWhiteSpace(legendName))
            boxesPlottable.LegendText = legendName;

        // If we want the right axis for this plottable, make the right axis visible and tell the plottable to use it
        if (rightAxis)
        {
            formsPlot.Plot.Axes.Right.IsVisible = true; // make the right axis visible
            // returned plottable type supports Axes property, so set YAxis to Right
            boxesPlottable.Axes.YAxis = formsPlot.Plot.Axes.Right;
        }

        // Configure X axis depending on grouping
        if (groups.Count > 0)
        {
            if (grouping == BoxGrouping.Category)
            {
                // numeric positions 1..N with labeled ticks
                var ticks = new Tick[groups.Count];
                for (int i = 0; i < groups.Count; i++)
                    ticks[i] = new Tick(i + 1, groups[i].Label);
                formsPlot.Plot.Axes.Bottom.TickGenerator = new NumericManual(ticks);
                // ensure left/right autoscaling
                formsPlot.Plot.Axes.AutoScale();
            }
            else
            {
                // DateTime axis on bottom
                formsPlot.Plot.Axes.DateTimeTicksBottom();
                // optionally AutoScale to include boxes
                formsPlot.Plot.Axes.AutoScale(); // note: AutoScale works on primary axes; if using right axis for Y, it will still correctly position X
            }
        }

        // Title, legend, dictionary entry, refresh
        if (!string.IsNullOrWhiteSpace(title))
            formsPlot.Plot.Title(title);

        if (dict is not null && !string.IsNullOrWhiteSpace(legendName))
            dict[legendName] = boxesPlottable;

        formsPlot.Refresh();

        return boxesPlottable;
    }

    // ---------- helper functions ----------

    private static (double Q1, double Median, double Q3, double WhiskerMin, double WhiskerMax) ComputeBoxStats(List<double> values)
    {
        double[] s = values.OrderBy(v => v).ToArray();
        int n = s.Length;
        double median = MedianOfArray(s);

        double[] lower, upper;
        if (n % 2 == 0)
        {
            lower = s.Take(n / 2).ToArray();
            upper = s.Skip(n / 2).ToArray();
        }
        else
        {
            lower = s.Take(n / 2).ToArray();                 // e.g. n=5 -> lower 2 elements
            upper = s.Skip(n / 2 + 1).ToArray();             // e.g. n=5 -> upper 2 elements
        }

        double q1 = MedianOfArray(lower);
        double q3 = MedianOfArray(upper);
        double iqr = q3 - q1;

        double lowerFence = q1 - 1.5 * iqr;
        double upperFence = q3 + 1.5 * iqr;

        double whiskerMin = s.Where(v => v >= lowerFence).DefaultIfEmpty(s.First()).First();
        double whiskerMax = s.Where(v => v <= upperFence).DefaultIfEmpty(s.Last()).Last();

        return (Q1: q1, Median: median, Q3: q3, WhiskerMin: whiskerMin, WhiskerMax: whiskerMax);
    }

    private static double MedianOfArray(double[] sorted)
    {
        if (sorted.Length == 0) return double.NaN;
        int n = sorted.Length;
        if (n % 2 == 1)
            return sorted[n / 2];
        else
            return (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
    }
    #endregion

    #region DataTable Group Helpers

    public static void FillByGroupNoLine<T>(string filter, IEnumerable<string> columnFiltersList, string columnGroup, string columnToAverage, FormsPlot formsPlot, DataTable table)
    {
        foreach (string fib in columnFiltersList)
        {
            var data = table.Select($"{filter} = '{fib}'");
            var xVal = data.AsEnumerable().Select(row => row.Field<DateTime>(columnGroup)).ToArray();
            var yVal = data.AsEnumerable().Select(row => Convert.ToDouble(row.Field<T>(columnToAverage))).ToArray();

            CreateScatterNoLine(xVal, yVal, formsPlot, fib);
        }

        formsPlot.Plot.Legend.FontSize = 8;
        formsPlot.Plot.ShowLegend();
    }

    public static void FillByGroupNoLine<T>(string filter, IEnumerable<string> columnFiltersList, FormsPlot formsPlot, DataTable table, Dictionary<string, ScatterSeriesWrapper> scatterDictionary)
    {
        foreach (string fib in columnFiltersList)
        {
            var data = table.Select($"{filter} = '{fib}'");
            var xVal = data.AsEnumerable().Select(row => row.Field<DateTime>(columnFiltersList.First())).ToArray();
            var yVal = data.AsEnumerable().Select(row => Convert.ToDouble(row.Field<T>(columnFiltersList.First()))).ToArray();

            var scatter = (Scatter)CreateScatterNoLine(xVal, yVal, formsPlot, fib);
            scatterDictionary[fib] = new ScatterSeriesWrapper(scatter, xVal, yVal);
        }

        formsPlot.Plot.Legend.FontSize = 8;
        formsPlot.Plot.ShowLegend();
    }

    public static void FillByGroup<T>(string columnFilter, IEnumerable<string> columnFiltersList, string columnGroup, string columnToAverage, FormsPlot formsPlot, DataTable table)
    {
        foreach (string fib in columnFiltersList)
        {
            var data = table.Select($"{columnFilter} = '{fib}'");
            var grouped = data.AsEnumerable().GroupBy(row => row.Field<string>(columnGroup)).ToArray();
            var xVal = grouped.Select(g => DateTime.ParseExact(g.Key!, "yyyyMMdd", CultureInfo.InvariantCulture)).ToArray();
            var yVal = grouped.Select(g => g.Average(row => Convert.ToDouble(row.Field<T>(columnToAverage)))).ToArray();

            CreateScatter(xVal, yVal, formsPlot, fib);
        }

        formsPlot.Plot.Legend.FontSize = 8;
        formsPlot.Plot.ShowLegend();
    }

    #endregion
    #region Statistics
    private static double Median(double[] values)
    {
        if (values == null || values.Length == 0)
            return double.NaN;

        var sorted = values.OrderBy(v => v).ToArray();
        int mid = sorted.Length / 2;
        return (sorted.Length % 2 == 0)
            ? (sorted[mid - 1] + sorted[mid]) / 2.0
            : sorted[mid];
    }

    private static double Quantile(double[] values, double q)
    {
        if (values == null || values.Length == 0)
            return double.NaN;
        if (q < 0 || q > 1)
            throw new ArgumentOutOfRangeException(nameof(q), "Quantile must be between 0 and 1.");

        var sorted = values.OrderBy(v => v).ToArray();
        double pos = (sorted.Length - 1) * q;
        int idx = (int)Math.Floor(pos);
        double frac = pos - idx;
        return (idx + 1 < sorted.Length)
            ? sorted[idx] * (1 - frac) + sorted[idx + 1] * frac
            : sorted[idx];
    }
    #endregion
}
