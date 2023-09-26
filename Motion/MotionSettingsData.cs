namespace ControllingAndManagingApp.Motion
{

    /// <summary>
    /// Represents motion settings data for a 3D printer, including maximum feedrates, default feedrates, accelerations, steps per unit,
    /// print speed, fan speed, print flow, home offsets, and G-code string generation methods.
    /// </summary>
    public class MotionSettingsData
    {
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
        /// <summary>
        /// Gets or sets the maximum acceleration for the extruder (E-axis).
        /// </summary>
        public int? EMaxAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the default acceleration for the X-axis.
        /// </summary>
        public int? XDefaultAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the default acceleration for the Y-axis.
        /// </summary>
        public int? YDefaultAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the default acceleration for the Z-axis.
        /// </summary>
        public int? ZDefaultAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the default acceleration for the extruder (E-axis).
        /// </summary>
        public int EDefaultAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the number of steps per unit.
        /// </summary>
        public int StepsPerUnit { get; set; }

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


        /// <summary>
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
        /// Generates a G-code string for the X-axis if the value is not null.
        /// </summary>
        /// <param name="x">The X-axis value.</param>
        /// <returns>The G-code string for the X-axis, or null if the value is null.</returns>
        public string XString(int? x)
        {
            if (x != null)
            {
                return $"X{x}";
            }
            return null;
        }

        /// <summary>
        /// Generates a G-code string for the Y-axis if the value is not null.
        /// </summary>
        /// <param name="y">The Y-axis value.</param>
        /// <returns>The G-code string for the Y-axis, or null if the value is null.</returns>
        public string YString(int? y)
        {
            if (y != null)
            {
                return $"Y{y}";
            }
            return null;
        }

        /// <summary>
        /// Generates a G-code string for the Z-axis if the value is not null.
        /// </summary>
        /// <param name="z">The Z-axis value.</param>
        /// <returns>The G-code string for the Z-axis, or null if the value is null.</returns>
        public string ZString(int? z)
        {
            if (z != null)
            {
                return $"Z{z}";
            }
            return null;
        }

        /// <summary>
        /// Generates a G-code string for the extruder (E-axis) if the value is not null.
        /// </summary>
        /// <param name="e">The extruder (E-axis) value.</param>
        /// <returns>The G-code string for the extruder (E-axis), or null if the value is null.</returns>
        public string EString(int? e)
        {
            if (e != null)
            {
                return $"E{e}";
            }
            return null;
        }

        /// <summary>
        /// Generates a G-code string for the X-axis if the value is not null.
        /// </summary>
        /// <param name="x">The X-axis value.</param>
        /// <returns>The G-code string for the X-axis, or null if the value is null.</returns>
        public string XString(double? x)
        {
            if (x != null)
            {
                return $"X{x}";
            }
            return null;
        }

        /// <summary>
        /// Generates a G-code string for the Y-axis if the value is not null.
        /// </summary>
        /// <param name="y">The Y-axis value.</param>
        /// <returns>The G-code string for the Y-axis, or null if the value is null.</returns>
        public string YString(double? y)
        {
            if (y != null)
            {
                return $"Y{y}";
            }
            return null;
        }

        /// <summary>
        /// Generates a G-code string for the Z-axis if the value is not null.
        /// </summary>
        /// <param name="z">The Z-axis value.</param>
        /// <returns>The G-code string for the Z-axis, or null if the value is null.</returns>
        public string ZString(double? z)
        {
            if (z != null)
            {
                return $"Z{z}";
            }
            return null;
        }

    }
}