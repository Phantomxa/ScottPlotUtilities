using MathNet.Numerics.LinearAlgebra;
using System.Runtime.CompilerServices;

namespace UT2_LISG_Stats
{
    internal static class Loess
    {
        public static double[] LoessSmooth(double[] x, double[] y, int bandwidth = 30)
        {
            if (x.Length != y.Length)
                throw new ArgumentException("x and y must be the same length");

            int n = x.Length;
            double[] ySmooth = new double[n];

            for (int i = 0; i < n; i++)
            {
                // 1. Get distances to all other points
                double[] distances = x.Select(xj => Math.Abs(xj - x[i])).ToArray();

                // 2. Get indices of the nearest neighbors
                int[] sortedIndices = Enumerable.Range(0, n)
                    .OrderBy(j => distances[j])
                    .Take(bandwidth)
                    .ToArray();

                // 3. Normalize distances and apply tricubic weights
                double maxDist = distances[sortedIndices.Last()];
                double[] weights = sortedIndices
                    .Select(j => Tricubic(distances[j] / maxDist))
                    .ToArray();

                // 4. Build design matrix (X) and response vector (Y)
                double[] xLocal = sortedIndices.Select(j => x[j]).ToArray();
                double[] yLocal = sortedIndices.Select(j => y[j]).ToArray();

                var X = Matrix<double>.Build.Dense(bandwidth, 2, (r, c) => c == 0 ? 1 : xLocal[r]);
                var W = Matrix<double>.Build.DenseDiagonal(bandwidth, bandwidth, r => weights[r]);
                var Y = Vector<double>.Build.Dense(yLocal);

                // 5. Solve the weighted least squares: β = (XᵀWX)^(-1) XᵀWY
                var XTW = X.TransposeThisAndMultiply(W);
                var beta = (XTW * X).Inverse() * XTW * Y;

                // 6. Predict at x[i]
                ySmooth[i] = beta[0] + beta[1] * x[i];
            }

            return ySmooth;
        }
        public static double[] OptimizedLoess(double[] x, double[] y, double bandwidth)
        {
            int n = x.Length;
            double[] output = new double[n];
            int windowSize = Math.Max(2, (int)(n * bandwidth));

            double[] sortedX = new double[n];
            double[] sortedY = new double[n];
            Array.Copy(x, sortedX, n);
            Array.Copy(y, sortedY, n);

            Array.Sort(sortedX, sortedY);

            for (int i = 0; i < n; i++)
            {
                double xi = x[i];

                int idx = Array.BinarySearch(sortedX, xi);
                if(idx < 0) { idx = ~idx; }

                int left = Math.Max(0, idx - windowSize / 2);
                int right = Math.Min(n - 1, left + windowSize - 1);
                left = Math.Max(0, right - windowSize + 1);

                double maxDist = Math.Abs(sortedX[right] - xi);

                if (maxDist == 0)
                {
                    output[i] = sortedY[idx];
                    continue;
                }

                double sumWeights = 0;
                double sumWX = 0;
                double sumWY = 0;
                double sumWXX = 0;
                double sumWXY = 0;

                for (int j = left; j <= right; j++)
                {
                    double xj = sortedX[j];
                    double yj = sortedY[j];
                    double dist = Math.Abs(xj - xi) / maxDist; // normalized distance
                    double weight = Tricubic(dist);

                    sumWeights += weight;
                    sumWX += weight * xj;
                    sumWY += weight * yj;
                    sumWXX += weight * xj * xj;
                    sumWXY += weight * xj * yj;
                }

                double denom = sumWeights * sumWXX - sumWX * sumWX;
                if (Math.Abs(denom) < 1e-10)
                {
                    output[i] = sumWY / sumWeights; // fall back to weighted average
                }
                else
                {
                    double beta = (sumWeights * sumWXY - sumWX * sumWY) / denom;
                    double alpha = (sumWY - beta * sumWX) / sumWeights;
                    output[i] = alpha + beta * xi;
                }
            }

            MessageBox.Show($"window size: {windowSize}");
            return output;
        }
        private static double Tricubic(double x)
        {
            if (x >= 1.0) return 0.0;
            double tmp = 1.0 - x * x * x;
            return tmp * tmp * tmp;
        }

    }
}
