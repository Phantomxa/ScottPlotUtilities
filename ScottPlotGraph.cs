using ScottPlot.WinForms;
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

namespace ScottPlotUtilities;

public static class ScottPlotHelpers
{
    #region Scatter / SignalXY / DataLogger

    public static IPlottable CreateScatter(DateTime[] xs, double[] ys, FormsPlot formsPlot,
        string legendName,
        Dictionary<string, IPlottable>? dict = null,
        Color? color = null,
        bool noLine = false,
        bool rightAxis = false)
    {
        var scatter = formsPlot.Plot.Add.Scatter(xs, ys);
        scatter.LegendText = legendName;
        if (color.HasValue) scatter.LineColor = color.Value;
        if (noLine) scatter.LineWidth = 0;
        if (rightAxis) scatter.Axes.YAxis = formsPlot.Plot.Axes.Right;
        dict[legendName] = scatter;

        formsPlot.Plot.Axes.DateTimeTicksBottom();
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
}
