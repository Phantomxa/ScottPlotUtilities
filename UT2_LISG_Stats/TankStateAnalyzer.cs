using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT2_LISG_Stats
{
    internal class TankStateAnalyzer
    {
        private enum State
        {
            INUSE,
            IDLE,
            FILL,
            OTHER,
            TOWERIDLE
        }
        public static DataTable AnalyzeTankStates(DataTable dataTable, string timestampColumn, List<string> tankSlopeColumns)
        {
            if (string.IsNullOrEmpty(timestampColumn) || tankSlopeColumns == null || tankSlopeColumns.Count == 0)
                throw new ArgumentException("Timestamp column and at least one tank slope column must be provided.");

            // Create a result table to hold each instance of tank state and their respective durations
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("Tank", typeof(string));
            resultTable.Columns.Add("State", typeof(string));
            resultTable.Columns.Add("StartTime", typeof(DateTime));
            resultTable.Columns.Add("EndTime", typeof(DateTime));
            resultTable.Columns.Add("Duration", typeof(TimeSpan));

            foreach (string slopeColumn in tankSlopeColumns)
            {
                string tankName = slopeColumn.Replace("Slope", ""); // Infer tank name from slope column name

                // Variables to track state
                string currentState = "IDLE";
                DateTime stateStartTime = DateTime.MinValue;
                DateTime stateEndTime;

                for (int i = 0; i < dataTable.Rows.Count - 1; i++)
                {
                    string newState;
                    DateTime currentTime = (DateTime)dataTable.Rows[i][timestampColumn];
                    DateTime nextTime = (DateTime)dataTable.Rows[i + 1][timestampColumn];
                    float slope = Convert.ToSingle(dataTable.Rows[i][slopeColumn]); // Slope column

                    // Determine the state based on slope thresholds or TimeSpan
                    if (IsTowerIdle(currentTime, nextTime))
                    {
                        newState = "TOWERIDLE";
                    }
                    else
                    {
                        newState = DetermineState(slope);
                    }

                    if (newState != currentState)
                    {
                        // Record the previous state instance
                        if (stateStartTime != DateTime.MinValue)
                        {
                            stateEndTime = currentTime;
                            TimeSpan duration = stateEndTime - stateStartTime;

                            resultTable.Rows.Add(tankName, currentState, stateStartTime, stateEndTime, duration);
                        }

                        // Update the state tracking variables
                        currentState = newState;
                        stateStartTime = currentTime;
                    }
                }

                // Record the final state instance
                if (stateStartTime != DateTime.MinValue)
                {
                    stateEndTime = (DateTime)dataTable.Rows[dataTable.Rows.Count - 1][timestampColumn];
                    TimeSpan duration = stateEndTime - stateStartTime;

                    resultTable.Rows.Add(tankName, currentState, stateStartTime, stateEndTime, duration);
                }
            }

            return resultTable;
        }

        public static DataTable CalculateMovingAverage(DataTable table, List<string> columnName, int windowSize)
        {
            if (windowSize <= 0)
                throw new ArgumentException("Window size must be greater than zero.", nameof(windowSize));

            foreach (string column in columnName)
            {
                if (!table.Columns.Contains(column))
                    throw new ArgumentException($"Column '{column}' does not exist in the DataTable.", nameof(column));

                // Add a new column to store the moving average
                string movingAvgColumnName = $"{column}_MovingAvg";
                if (!table.Columns.Contains(movingAvgColumnName))
                    table.Columns.Add(movingAvgColumnName, typeof(float));

                Queue<double> window = new Queue<double>();
                double windowSum = 0;

                foreach (DataRow row in table.Rows)
                {
                    float currentValue = Convert.ToSingle(row[column]);

                    // Add the current value to the sliding window
                    window.Enqueue(currentValue);
                    windowSum += currentValue;

                    // Maintain the sliding window size
                    if (window.Count > windowSize)
                    {
                        windowSum -= window.Dequeue();
                    }

                    // Calculate the moving average and store it in the new column
                    row[movingAvgColumnName] = windowSum / window.Count;
                }
            }

            return table;
        }
        public static DataTable CalculateSlope(DataTable table, List<string> columnName, int windowSize, string timeColumnName)
        {
            if (windowSize <= 0)
                throw new ArgumentException("Window size must be greater than zero.", nameof(windowSize));

            foreach (string column in columnName)
            {
                if (!table.Columns.Contains(column))
                    throw new ArgumentException($"Column '{column}' does not exist in the DataTable.", nameof(column));

                // Add a new column to store the moving average
                string slopeColumnName = $"Y_Slope";
                if (!table.Columns.Contains(slopeColumnName))
                    table.Columns.Add(slopeColumnName, typeof(float));

                Queue<double> window = new Queue<double>();
                Queue<DateTime> windowTime = new Queue<DateTime>();

                foreach (DataRow row in table.Rows)
                {

                    float currentValue = Convert.ToSingle(row[column]);
                    DateTime currentTime = DateTime.FromOADate((double)row[timeColumnName]);

                    window.Enqueue(currentValue);
                    windowTime.Enqueue(currentTime);

                    TimeSpan span = currentTime - windowTime.Peek();
                    var rise = currentValue - window.Peek();

                    // Maintain the sliding window size
                    if (span.TotalMinutes >= windowSize)
                    {
                        window.Dequeue();
                        windowTime.Dequeue();
                    }

                    // Calculate the rate of change per windowsize minute and store it in the new column
                    row[slopeColumnName] = ((rise) / (span.TotalMinutes)) * windowSize;

                }
            }

            return table;
        }
        public static double[] CalculateSlope(double[] values, DateTime[] timestamps, int windowMinutes)
        {
            if (values == null || timestamps == null)
                throw new ArgumentNullException("Input arrays cannot be null.");
            if (values.Length != timestamps.Length)
                throw new ArgumentException("Values and timestamps must be the same length.");
            if (windowMinutes <= 0)
                throw new ArgumentException("Window size must be greater than zero.", nameof(windowMinutes));

            int n = values.Length;
            double[] slopes = new double[n];

            Queue<double> windowValues = new Queue<double>();
            Queue<DateTime> windowTimes = new Queue<DateTime>();

            for (int i = 0; i < n; i++)
            {
                double currentValue = values[i];
                DateTime currentTime = timestamps[i];

                windowValues.Enqueue(currentValue);
                windowTimes.Enqueue(currentTime);

                // Remove points outside the time window
                while (windowTimes.Count > 0 && (currentTime - windowTimes.Peek()).TotalMinutes >= windowMinutes)
                {
                    windowValues.Dequeue();
                    windowTimes.Dequeue();
                }

                // Calculate slope (rate of change)
                if (windowValues.Count >= 2)
                {
                    double rise = currentValue - windowValues.Peek();
                    double runMinutes = (currentTime - windowTimes.Peek()).TotalMinutes;

                    slopes[i] = runMinutes != 0 ? (rise / runMinutes) * windowMinutes : 0.0;
                }
                else
                {
                    slopes[i] = 0.0; // Not enough data to calculate a slope
                }
            }

            return slopes;
        }

        public static List<double> CalculateSlopeToList(DataTable table, List<string> columnName, int windowSize)
        {
            if (windowSize <= 0)
                throw new ArgumentException("Window size must be greater than zero.", nameof(windowSize));

            List<double> slopeList = new List<double>();

            foreach (string column in columnName)
            {
                if (!table.Columns.Contains(column))
                    throw new ArgumentException($"Column '{column}' does not exist in the DataTable.", nameof(column));

                // Add a new column to store the moving average
                string slopeColumnName = $"{column}_Slope";
                if (!table.Columns.Contains(slopeColumnName))
                    table.Columns.Add(slopeColumnName, typeof(float));

                Queue<double> window = new Queue<double>();
                Queue<DateTime> windowTime = new Queue<DateTime>();

                foreach (DataRow row in table.Rows)
                {

                    float currentValue = Convert.ToSingle(row[column]);
                    DateTime currentTime = DateTime.FromOADate((double)row["X"]);

                    window.Enqueue(currentValue);
                    windowTime.Enqueue(currentTime);

                    TimeSpan span = currentTime - windowTime.Peek();
                    var rise = currentValue - window.Peek();

                    // Maintain the sliding window size
                    if (span.TotalMinutes >= windowSize)
                    {
                        window.Dequeue();
                        windowTime.Dequeue();
                    }

                    // Calculate the rate of change per windowsize minute and store it in the new column
                    row[slopeColumnName] = ((rise) / (span.TotalMinutes)) * windowSize;
                    slopeList.Add(((rise) / (span.TotalMinutes)) * windowSize);

                }
            }

            return slopeList;
        }

        public static DataTable GetFillTime(DataTable dt, List<string> columnName, int windowSize)
        {
            // Can improve by only copying columns that are being analyzed
            DataTable table = dt.Copy();
            string tankstate;

            if (windowSize <= 0)
                throw new ArgumentException("Window size must be greater than zero.", nameof(windowSize));

            foreach (string column in columnName)
            {
                int counter = 0;

                if (!table.Columns.Contains(column))
                    throw new ArgumentException($"Column '{column}' does not exist in the DataTable.", nameof(column));

                // Add a new column to store the state
                string state = $"{column} State";
                if (!table.Columns.Contains(state))
                    table.Columns.Add(state, typeof(string));

                Queue<double> window = new Queue<double>();
                Queue<DateTime> windowTime = new Queue<DateTime>();
                double windowSum = 0;

                foreach (DataRow row in table.Rows)
                {
                    if (counter < 1)
                    {
                        counter++;
                        continue;
                    }

                    float currentValue = Convert.ToSingle(row[column]);
                    DateTime currentTime = Convert.ToDateTime(row["t_stamp"]);

                    window.Enqueue(currentValue);
                    windowTime.Enqueue(currentTime);

                    if (float.IsNaN(currentValue))
                    {
                        continue;
                    }
                    else
                    {
                        windowSum += currentValue;
                    }

                    TimeSpan span = currentTime - windowTime.Peek();
                    var rise = currentValue - window.Peek();

                    if (counter < (windowSize - 1))
                    {
                        counter++;
                        continue;
                    }

                    // Maintain the sliding window size
                    if (window.Count > windowSize)
                    {
                        windowSum -= window.Dequeue();
                    }

                    // Calculate the moving average and store it in the new column
                    var average = windowSum / window.Count;

                    if (Math.Round(average, 1) <= -0.3)
                    {
                        row[state] = "INUSE";
                        tankstate = "INUSE";
                    }
                    else if (Math.Round(average, 1) < 0.2 && Math.Round(average, 1) > -0.2)
                    {
                        row[state] = "IDLE";
                        tankstate = "IDLE";
                    }
                    else if (Math.Round(average, 1) >= 0.3)
                    {
                        row[state] = "FILL";
                        tankstate = "FILL";
                    }
                    else
                    {
                        row[state] = "other";
                        tankstate = "other";
                    }
                    counter++;
                }
            }

            return table;
        }
        private static string DetermineState(float slope)
        {
            if (Math.Round(slope, 1) <= -0.3)
            {
                return State.INUSE.ToString(); ;
            }
            else if (Math.Round(slope, 1) <= 0.2 && Math.Round(slope, 1) >= -0.2)
            {
                return State.IDLE.ToString();
            }
            else if (Math.Round(slope, 1) >= 0.3)
            {
                return State.FILL.ToString();
            }
            else
            {
                return State.OTHER.ToString();
            }
        }
        private static bool IsTowerIdle(DateTime previousTime, DateTime currentTime)
        {
            double span = currentTime.Subtract(previousTime).TotalMinutes;
            if (span > 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static string DetermineTransition(string currentState, string newState)
        {
            // FINISH THIS AFTER IsTowerIdle() method is complete
            if (currentState != newState)
            {
                if (currentState == "INUSE" & newState == "IDLE")
                {
                    return "SWITCH";
                }
            }

            return newState;
        }
    }
}
