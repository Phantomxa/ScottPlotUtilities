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
        public static void ThemeMinimal(this ScottPlot.WinForms.FormsPlot formsPlot,
        string? title = null, float? titleSize = null,
        string? leftTitle = null, float? leftSize = null,
        string? rightTitle = null, float? rightSize = null,
        string? bottomTitle = null, float? bottomSize = null)
        {
            var plot = formsPlot.Plot;

            ScottPlot.Color minimalColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromArgb(235, 235, 235));
            ScottPlot.Color axesFrameColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromArgb(255, 255, 255));

            plot.Axes.FrameColor(axesFrameColor);
            plot.Grid.XAxisStyle.MajorLineStyle.Color = minimalColor;
            plot.Grid.YAxisStyle.MajorLineStyle.Color = minimalColor;
            plot.Grid.XAxisStyle.MajorLineStyle.Width = 1;

            foreach (var yAx in plot.Axes.GetYAxes())
            {
                yAx.FrameLineStyle.IsVisible = true;
                yAx.MinorTickStyle.Color = axesFrameColor;
                yAx.MajorTickStyle.Color = axesFrameColor;

                if (yAx.Edge == ScottPlot.Edge.Left && leftTitle != null)
                {
                    yAx.Label.Text = leftTitle;
                    if (leftSize.HasValue) yAx.Label.FontSize = leftSize.Value;
                }
                else if (yAx.Edge == ScottPlot.Edge.Right && rightTitle != null)
                {
                    yAx.Label.Text = rightTitle;
                    if (rightSize.HasValue) yAx.Label.FontSize = rightSize.Value;
                }
            }

            plot.Axes.Bottom.MajorTickStyle.Color = axesFrameColor;
            if (bottomTitle != null) plot.Axes.Bottom.Label.Text = bottomTitle;
            if (bottomSize.HasValue) plot.Axes.Bottom.Label.FontSize = bottomSize.Value;

            if (title != null)
            {
                plot.Title(title, titleSize ?? 16);
            }

            formsPlot.Refresh();
        }
    }
}
