using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WinForms;

namespace UT2_LISG_Stats
{
    /// <summary>
    /// Tracks mouse movement over Scatter or SignalXY plots on both primary and secondary Y axes.
    /// Highlights the nearest point with a crosshair.
    /// </summary>
    public class HoverManager
    {
        private readonly FormsPlot formsPlot;
        private readonly Crosshair crosshair;
        private readonly Crosshair crosshair2;

        /// <summary>
        /// If true, finds nearest point using both X and Y coordinates.
        /// If false, uses X only.
        /// </summary>
        public bool NearestXY { get; set; } = true;

        /// <summary>
        /// Event raised when the mouse hovers a valid point.
        /// Provides (plottable, index, x, y) of the selected point.
        /// </summary>
        public event Action<IPlottable, int, double, double>? PointHovered;

        /// <summary>
        /// Event raised when the mouse is not hovering any point.
        /// </summary>
        public event Action? NoPointHovered;

        public HoverManager(FormsPlot plot)
        {
            formsPlot = plot ?? throw new ArgumentNullException(nameof(plot));

            crosshair = formsPlot.Plot.Add.Crosshair(0, 0);
            crosshair.IsVisible = false;
            crosshair.MarkerShape = MarkerShape.OpenCircle;
            crosshair.MarkerSize = 15;

            crosshair2 = formsPlot.Plot.Add.Crosshair(0, 0);
            crosshair2.IsVisible = false;
            crosshair2.MarkerShape = MarkerShape.OpenCircle;
            crosshair2.MarkerSize = 15;
            crosshair2.Axes.YAxis = formsPlot.Plot.Axes.Right;

            formsPlot.MouseMove += FormsPlot_MouseMove;
        }

        private void FormsPlot_MouseMove(object? sender, MouseEventArgs e)
        {
            Pixel mousePixel = new(e.Location.X, e.Location.Y);

            IPlottable? bestPlottable = null;
            DataPoint bestPoint = default;
            double bestPixelDist = double.PositiveInfinity;

            foreach (var plottable in formsPlot.Plot.GetPlottables())
            {
                if (plottable.GetType().Name == "Crosshair" | plottable.GetType().Name == "Annotation")
                {
                    continue;
                }
                DataPoint nearest = default;

                // Convert mouse pixel → coordinates (no renderInfo needed in 5.0.55)
                Coordinates mouseCoords = plottable.Axes.GetCoordinates(mousePixel);
                Coordinates mouseCoords2 = formsPlot.Plot.GetCoordinates(mousePixel);
                switch (plottable)
                {
                    case Scatter scatter:
                        nearest = NearestXY
                            ? scatter.Data.GetNearest(mouseCoords, formsPlot.Plot.LastRender)
                            : scatter.Data.GetNearestX(mouseCoords, formsPlot.Plot.LastRender);
                        break;

                    case SignalXY sigXY:
                        nearest = NearestXY
                            ? sigXY.Data.GetNearest(mouseCoords, formsPlot.Plot.LastRender)
                            : sigXY.Data.GetNearestX(mouseCoords, formsPlot.Plot.LastRender);
                        break;

                    default:
                        continue;
                }

                if (nearest.IsReal)
                {
                    // distance in data coordinates (works for both primary and secondary axes)
                    // double dx = mouseCoords.X - nearest.X;
                    // double dy = mouseCoords.Y - nearest.Y;
                    // double dist = Math.Sqrt(dx * dx + dy * dy);

                    // Convert back to pixel space (no renderInfo needed in 5.0.55)
                    Pixel nearestPixel = plottable.Axes.GetPixel(nearest.Coordinates);
                    double dist = nearestPixel.DistanceFrom(mousePixel);

                    if (dist < bestPixelDist)
                    {
                        bestPixelDist = dist;
                        bestPoint = nearest;
                        bestPlottable = plottable;
                    }

                    
                }
            }

            if (bestPlottable is not null)
            {
                crosshair.IsVisible = true;
                crosshair.Position = bestPoint.Coordinates;
                crosshair2.IsVisible = true;
                crosshair2.Position = bestPoint.Coordinates;
                formsPlot.Refresh();

                PointHovered?.Invoke(bestPlottable, bestPoint.Index, bestPoint.X, bestPoint.Y);
            }
            else if (crosshair.IsVisible)
            {
                crosshair.IsVisible = false;
                crosshair2.IsVisible = false;
                formsPlot.Refresh();
                NoPointHovered?.Invoke();
            }
        }
    }
}
