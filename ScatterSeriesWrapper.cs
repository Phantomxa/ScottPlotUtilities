using ScottPlot;

namespace ScottPlotUtilities
{
    public class ScatterSeriesWrapper
    {
        public IPlottable Scatter { get; set; }
        public DateTime[] XsOriginal { get; set; } = Array.Empty<DateTime>();
        public double[] YsOriginal { get; set; } = Array.Empty<double>();
        public ScatterSeriesWrapper(IPlottable scatter, DateTime[] xValues, double[] yValues)
        {
            Scatter = scatter;
            XsOriginal = xValues;
            YsOriginal = yValues;
        }
    }
}
