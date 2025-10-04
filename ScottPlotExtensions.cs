using ScottPlot.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = ScottPlot.Color;

namespace ScottPlotUtilities
{
    public static class ScottPlotExtensions
    {
        public static void DarkMode(this FormsPlot formsPlot)
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
    }
}
