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

namespace ScottPlotUtilities
{
    internal class ScottPlotGraph
    {
        public static void FillScottByGroup<T>(string columnFilter, IEnumerable<string> columnFiltersList, string columnGroup, string columnToAverage, FormsPlot formsPlot, DataTable table)
        {
            foreach (string fib in columnFiltersList)
            {
                var data = table.Select($"{columnFilter} = '{fib}'");
                var xVal = data
                    .AsEnumerable()
                    .GroupBy(row => row.Field<string>(columnGroup))
                    .Select(x => DateTime.ParseExact(x.Key!, "yyyyMMdd", CultureInfo.InvariantCulture))
                   //.Select(x => DateTime.ParseExact(x.Field<string>("draw_date"), "yyyyMMdd", CultureInfo.InvariantCulture))
                   // .OrderBy(x => x)
                   .ToArray();
                var yVal = data
                    .AsEnumerable()
                    .GroupBy(row => row.Field<string>(columnGroup))
                    //.Select(y => (double)y.Field<decimal>("bksRt"))
                    .Select(y => y.Average(row => Convert.ToDouble(row.Field<T>(columnToAverage))))
                    .ToArray();

                ScottPlotGraph.CreateScottScatter(xVal, yVal, formsPlot, fib);
            }

            formsPlot.Plot.Legend.FontSize = 8;
            formsPlot.Plot.ShowLegend();
        }
        public static void FillScott(FormsPlot formsPlot, DataTable dat, DataTable checkIfThisTableIsNullToProceed)
        {
            if (checkIfThisTableIsNullToProceed is not null)
            {
                var xVal = dat.AsEnumerable().Select(x => x.Field<DateTime>("draw_date")).ToArray();
                var yVal = dat.AsEnumerable().Select(y => y.Field<double>("AvgBreakRate")).ToArray();
                ScottPlotGraph.CreateScottScatter(xVal, yVal, formsPlot, "AvgBreakRate");

                formsPlot.Plot.Legend.FontSize = 8;
                formsPlot.Plot.ShowLegend();
            }
            else
            {
                MessageBox.Show("expLenData1 talbe is null. Cannot fill Plot 2!");
            }
        }
        public static void FillScott(FormsPlot formsPlot, DataTable dat, DataTable checkIfThisTableIsNullToProceed, ScottPlot.Color color)
        {
            if (checkIfThisTableIsNullToProceed is not null)
            {
                var xVal = dat.AsEnumerable().Select(x => x.Field<DateTime>("draw_date")).ToArray();
                var yVal = dat.AsEnumerable().Select(y => y.Field<double>("AvgBreakRate")).ToArray();
                CreateScottScatter(xVal, yVal, formsPlot, "AvgBreakRate", color);

                formsPlot.Plot.Legend.FontSize = 8;
                formsPlot.Plot.ShowLegend();
            }
            else
            {
                MessageBox.Show("expLenData1 talbe is null. Cannot fill Plot 2!");
            }
        }
        public static void FillScottSecondary(FormsPlot formsPlot, DataTable dat, string legendName, DataTable checkIfThisTableIsNullToProceed)
        {
            if (checkIfThisTableIsNullToProceed is not null)
            {
                var xVal = dat.AsEnumerable().Select(x => x.Field<DateTime>("draw_date")).ToArray();
                var yVal = dat.AsEnumerable().Select(y => y.Field<double>("off_bksRt")).ToArray();
                ScottPlotGraph.CreateScottSecondary(xVal, yVal, formsPlot, legendName);

                formsPlot.Plot.Legend.FontSize = 8;
                //formsPlot.Plot.Add.HorizontalLine(150.0, 2, Colors.Red, ScottPlot.LinePattern.Dashed);
                formsPlot.Plot.ShowLegend();
            }
            else
            {
                MessageBox.Show("expLenData1 table is null. Cannot fill Plot 2!");
            }
        }
        public static void FillScottSecondary(FormsPlot formsPlot, DataTable dat, string legendName, DataTable checkIfThisTableIsNullToProceed, ScottPlot.Color color)
        {
            if (checkIfThisTableIsNullToProceed is not null)
            {
                var xVal = dat.AsEnumerable().Select(x => x.Field<DateTime>("draw_date")).ToArray();
                var yVal = dat.AsEnumerable().Select(y => y.Field<double>("off_bksRt")).ToArray();
                ScottPlotGraph.CreateScottSecondary(xVal, yVal, formsPlot, legendName, color);

                formsPlot.Plot.Legend.FontSize = 8;
                //formsPlot.Plot.Add.HorizontalLine(150.0, 2, Colors.Red, ScottPlot.LinePattern.Dashed);
                formsPlot.Plot.ShowLegend();
            }
            else
            {
                MessageBox.Show("expLenData1 table is null. Cannot fill Plot 2!");
            }
        }
        public static void CreateScottScatter(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName)
        {
            var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
            scatter.LegendText = legendName;
            formsPlot.Plot.Axes.DateTimeTicksBottom();
            formsPlot.Refresh();
        }
        public static void CreateScottScatter(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, ScottPlot.Color color)
        {
            var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
            scatter.Color = color;
            scatter.LegendText = legendName;
            formsPlot.Plot.Axes.DateTimeTicksBottom();
            formsPlot.Refresh();
        }
        public static void CreateScottScatterNoLine(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName)
        {
            var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
            scatter.LineWidth = 0;
            scatter.LegendText = legendName;
            formsPlot.Plot.Axes.DateTimeTicksBottom();

            formsPlot.Plot.Legend.FontSize = 8;
            formsPlot.Plot.ShowLegend();

            formsPlot.Refresh();
        }
        public static ScottPlot.Plottables.Scatter ReturnScottScatterNoLine(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName)
        {
            var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
            scatter.LineWidth = 0;
            scatter.LegendText = legendName;
            formsPlot.Plot.Axes.DateTimeTicksBottom();
            formsPlot.Refresh();

            return scatter;
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
        public static void CreateScottScatterNoLineUpdateIPlottable(DateTime[] xs, double[] ydata, FormsPlot formsPlot, string legendName, Dictionary<string, ScatterSeriesWrapper> dictionary)
        {
            var scatter = formsPlot.Plot.Add.Scatter(xs, ydata);
            scatter.LineWidth = 0;
            scatter.LegendText = legendName;
            formsPlot.Plot.Axes.DateTimeTicksBottom();
            //dictionary[legendName] = new ScatterSeriesWrapper(scatter, xs, ydata); CHANGE - Overwrites the original data.
            dictionary[legendName].Scatter = scatter; // ONLY update the IPlottable object and leave original data alone
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
        public static void CreateScottSecondary(DateTime[] myDates, double[] ydata, FormsPlot formsPlot, string legendName, ScottPlot.Color color)
        {
            //var yAxis3 = formsPlot.Plot.Axes.AddRightAxis(); // Do this if want a second Right axis (so 2 of them on the right)
            var sigBig = formsPlot.Plot.Add.Scatter(myDates, ydata);
            sigBig.Color = color;
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
        public static void CreateScottSecondary(DateTime[] myDates, double[] ydata, FormsPlot formsPlot, string legendName, string secondaryAxisName, ScottPlot.Color color)
        {
            //var yAxis3 = formsPlot.Plot.Axes.AddRightAxis(); // Do this if want a second Right axis (so 2 of them on the right)
            var sigBig = formsPlot.Plot.Add.Scatter(myDates, ydata);
            sigBig.Color = color;
            sigBig.Axes.YAxis = formsPlot.Plot.Axes.Right;
            sigBig.LegendText = legendName;
            formsPlot.Plot.Axes.Right.Label.Text = secondaryAxisName;
            formsPlot.Refresh();
        }
        public static void FillScottByGroupCountBar(Dictionary<string?, int> pieValues, FormsPlot formsPlot)
        {
            var counter = 1;
            var ticks = new Tick[pieValues.Count];
            foreach (var item in pieValues)
            {
                var val = item.Value;
                var lab = item.Key;
                var bar = formsPlot.Plot.Add.Bar(counter, val);
                bar.Horizontal = true;

                ticks[counter - 1] = new Tick(counter, lab);

                counter++;
            }

            formsPlot.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            formsPlot.Plot.Axes.Left.MajorTickStyle.Length = 0;
            formsPlot.Plot.Axes.Margins(left: 0);
            formsPlot.Plot.HideGrid();
            formsPlot.Plot.Axes.AutoScale();
            formsPlot.Refresh();
        }
        public static void FillScottByGroupCountPareto(Dictionary<string?, int> pieValues, FormsPlot formsPlot)
        {
            var orderedValues = pieValues.AsEnumerable().OrderBy(x => x.Value);

            var counter = 1;
            var ticks = new Tick[orderedValues.Count()];
            foreach (var item in orderedValues)
            {
                var val = item.Value;
                var lab = item.Key;
                var bar = formsPlot.Plot.Add.Bar(counter, val);
                bar.Horizontal = true;

                ticks[counter - 1] = new Tick(counter, lab);

                counter++;
            }

            formsPlot.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            formsPlot.Plot.Axes.Left.MajorTickStyle.Length = 0;
            formsPlot.Plot.Axes.Margins(left: 0);
            formsPlot.Plot.HideGrid();
            formsPlot.Plot.Axes.AutoScale();
            formsPlot.Refresh();
        }
        public static void FillScottByGroupNoLine<T>(string filter, IEnumerable<string> columnFiltersList, string columnGroup, string columnToAverage, FormsPlot formsPlot, DataTable table)
        {
            foreach (string fib in columnFiltersList)
            {
                var data = table.Select($"{filter} = '{fib}'");
                var xVal = data
                    .AsEnumerable().
                    Select(cols => cols.Field<DateTime>("DrawDate")).
                    ToArray();
                var yVal = data
                    .AsEnumerable()
                    .Select(y => Convert.ToDouble(y.Field<T>("mach_no")))
                    .ToArray();

                ScottPlotGraph.CreateScottScatterNoLine(xVal, yVal, formsPlot, fib);
            }

            formsPlot.Plot.Legend.FontSize = 8;
            formsPlot.Plot.ShowLegend();
        }
        public static void FillScottByGroupNoLine<T>(string filter, IEnumerable<string> columnFiltersList, FormsPlot formsPlot, DataTable table, Dictionary<string, ScatterSeriesWrapper> scatterDictionary)
        {
            foreach (string fib in columnFiltersList)
            {
                var data = table.Select($"{filter} = '{fib}'");
                var xVal = data
                    .AsEnumerable().
                    Select(cols => cols.Field<DateTime>("DrawDate")).
                    ToArray();
                var yVal = data
                    .AsEnumerable()
                    .Select(y => Convert.ToDouble(y.Field<T>("mach_no")))
                    .ToArray();

                //var scatter = ScottPlotGraph.CreateScottScatterNoLine(xVal, yVal, formsPlot, fib, seriesDict);
                var scatter = ReturnScottScatterNoLine(xVal, yVal, formsPlot, fib);
                scatterDictionary[fib] = new ScatterSeriesWrapper(scatter, xVal, yVal);
            }

            formsPlot.Plot.Legend.FontSize = 8;
            formsPlot.Plot.ShowLegend();
        }
    }
}
