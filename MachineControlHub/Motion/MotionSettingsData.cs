namespace MachineControlHub.Motion
{

    /// <summary>
    /// Represents motion settings data for a 3D printer, including maximum feedrates, default feedrates, accelerations, steps per unit,
    /// print speed, fan speed, print flow, home offsets, and G-code string generation methods.
    /// </summary>
    public class MotionSettingsData
    {
        public enum Axis { X, Y, Z, E }

        public int? MinTravelFeedrate { get; set; }

        public int? MinPrintFeedrate { get; set; }

        public int? XMaxJerk { get; set; }

        public int? YMaxJerk { get; set; }

        public int? ZMaxJerk { get; set; }

        public int? EMaxJerk { get; set; }

        public int? MinSegmentTime { get; set; }

        public double? JunctionDeviation { get; set; }

        public int? FadeHeight { get; set; }

        /// <summary>
        /// Gets or sets the maximum feedrate for the X-axis.
        /// </summary>
        public int? XMaxFeedrate { get; set; }

        /// <summary>
        /// Gets or sets the maximum feedrate for the Y-axis.
        /// </summary>
        public int? YMaxFeedrate { get; set; }

        /// <summary>
        /// Gets or sets the maximum feedrate for the Z-axis.
        /// </summary>
        public int? ZMaxFeedrate { get; set; }

        /// <summary>
        /// Gets or sets the maximum feedrate for the extruder.
        /// </summary>
        public int? EMaxFeedrate { get; set; }

        /// <summary>
        /// Gets or sets the default feedrate for the X-axis.
        /// </summary>
        public int? XDefaultFeedrate { get; set; }

        /// <summary>
        /// Gets or sets the default feedrate for the Y-axis.
        /// </summary>
        public int? YDefaultFeedrate { get; set; }

        /// <summary>
        /// Gets or sets the default feedrate for the Z-axis.
        /// </summary>
        public int? ZDefaultFeedrate { get; set; }

        /// <summary>
        /// Gets or sets the default feedrate for the extruder.
        /// </summary>
        public int? EDefaultFeedrate { get; set; }

        /// <summary>
        /// Gets or sets the maximum acceleration for the X-axis.
        /// </summary>
        public int? XMaxAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the maximum acceleration for the Y-axis.
        /// </summary>
        public int? YMaxAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the maximum acceleration for the Z-axis.
        /// </summary>
        public int? ZMaxAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the maximum acceleration for the extruder (E-axis).
        /// </summary>
        public int? EMaxAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the default acceleration for the X-axis.
        /// </summary>
        public int? PrintAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the default acceleration for the Y-axis.
        /// </summary>
        public int? RetractAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the default acceleration for the Z-axis.
        /// </summary>
        public int? TravelAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the number of steps per unit.
        /// </summary>
        public int? XStepsPerUnit { get; set; }

        public int? YStepsPerUnit { get; set; }

        public int? ZStepsPerUnit { get; set; }

        public int? EStepsPerUnit { get; set; }

        /// <summary>
        /// Gets or sets the feedrate for free moves.
        /// </summary>
        public int? FeedRateFreeMove { get; set; }

        /// <summary>
        /// Gets or sets the print speed.
        /// </summary>
        public double PrintSpeed { get; set; }

        /// <summary>
        /// Gets or sets the fan speed.
        /// </summary>
        public int FanSpeed { get; set; }

        /// <summary>
        /// Gets or sets the print flow.
        /// </summary>
        public int PrintFlow { get; set; }

        /// <summary>
        /// Gets or sets the X-axis home offset.
        /// </summary>
        public double? XHomeOffset { get; set; }

        /// <summary>
        /// Gets or sets the Y-axis home offset.
        /// </summary>
        public double? YHomeOffset { get; set; }

        /// <summary>
        /// Gets or sets the Z-axis home offset.
        /// </summary>
        public double? ZHomeOffset { get; set; }

        /// <summary>
        /// Gets or sets the X-axis home position.
        /// </summary>
        public double XHomePos { get; set; }

        /// <summary>
        /// Gets or sets the Y-axis home position.
        /// </summary>
        public double YHomePos { get; set; }

        /// <summary>
        /// Gets or sets the Z-axis home position.
        /// </summary>
        public double ZHomePos { get; set; }

        public int? PlannerFrequencyLimit { get; set; }

        public int? PlannerXYFrequencyMinimumSpeedPercentage { get; set; }

        public int? TargetExtruder { get; set; }

        /// Generates a G-code string for the feed rate if the value is not null.
        /// </summary>
        /// <param name="x">The feed rate value.</param>
        /// <returns>The G-code string for the feed rate, or null if the value is null.</returns>
        public string FeedRateString(int? x)
        {
            if (x != null)
            {
                return $"F{x}";
            }
            return null;
        }


        /// <summary>
        /// Generates a G-code string for a specified axis and its corresponding value.
        /// </summary>
        /// <param name="axis">The axis for which to generate the G-code (e.g., X, Y, Z, etc.).</param>
        /// <param name="value">The value associated with the axis.</param>
        /// <returns>
        /// A G-code string representing the specified axis and its value, or null if the value is null.
        /// </returns>
        public static string AxisString(Axis axis, object value)
        {
            if (value == null)
            {
                return null;
            }

            string axisCode = axis.ToString();
            string valueString = value.ToString();

            return $"{axisCode}{valueString}";
        }

    }
}