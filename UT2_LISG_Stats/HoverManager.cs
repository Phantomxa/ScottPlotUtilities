using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WinForms;
using System.Numerics;

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
                    case DataLogger logger:
                        {
                            double[] xs = logger.Data.Coordinates.Select(c => c.X).ToArray(); ;
                            double[] ys = logger.Data.Coordinates.Select(c => c.Y).ToArray(); ;

                            if (xs.Length == 0)
                                continue;

                            int nearestIndex = ClosestIndex(xs, mouseCoords.X);
                            if (nearestIndex < 0 || nearestIndex >= ys.Length)
                                continue;

                            double nearestX = xs[nearestIndex];
                            double nearestY = ys[nearestIndex];

                            nearest = new(nearestX, nearestY, nearestIndex);

                            break;
                        }

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

        /// <summary>
        /// Finds the index of the value in xs that is closest to target.
        /// Returns -1 if xs is null or empty.
        /// </summary>
        private static int ClosestIndex(double[] xs, double target)
        {
            if (xs == null || xs.Length == 0)
                return -1;

            int bestIndex = 0;
            double bestDist = Math.Abs(xs[0] - target);

            for (int i = 1; i < xs.Length; i++)
            {
                double dist = Math.Abs(xs[i] - target);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

    }
}
