namespace MachineControlHub.Motion

{
    /// <summary>
    /// Represents the available move positions.
    /// </summary>
    public enum MovePositions
    {
        /// <summary>
        /// Represents the X-axis move position.
        /// </summary>
        XMovePos,

        /// <summary>
        /// Represents the Y-axis move position.
        /// </summary>
        YMovePos,

        /// <summary>
        /// Represents the Z-axis move position.
        /// </summary>
        ZMovePos,

        /// <summary>
        /// Represents the extruder (E) move position.
        /// </summary>
        EMovePos
    }

    public enum CurrentPositions
    {
        /// <summary>
        /// Represents the X-axis current position.
        /// </summary>
        XCurrentPos,

        /// <summary>
        /// Represents the Y-axis current position.
        /// </summary>
        YCurrentPos,

        /// <summary>
        /// Represents the Z-axis current position.
        /// </summary>
        ZCurrentPos,

        /// <summary>
        /// Represents the E-axis current position.
        /// </summary>
        ECurrentPos,
    }


    /// <summary>
    /// Represents a position in 3D space, including current and optional move positions.
    /// </summary>
    public class Position
    {

        /// <summary>
        /// Gets or sets the current X-axis position.
        /// </summary>
        public double XCurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the current Y-axis position.
        /// </summary>
        public double YCurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the current Z-axis position.
        /// </summary>
        public double ZCurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the current extruder (E) position.
        /// </summary>
        public double ECurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the optional X-axis move position.
        /// </summary>
        public double? XMovePosition { get; set; }

        /// <summary>
        /// Gets or sets the optional Y-axis move position.
        /// </summary>
        public double? YMovePosition { get; set; }

        /// <summary>
        /// Gets or sets the optional Z-axis move position.
        /// </summary>
        public double? ZMovePosition { get; set; }

        /// <summary>
        /// Gets or sets the optional extruder (E) move position.
        /// </summary>
        public double? EMovePosition { get; set; }


        /// <summary>
        /// Generates a move string based on the specified move position.
        /// </summary>
        /// <param name="position">The desired move position (X, Y, Z, or E).</param>
        /// <returns>A formatted move string (e.g., "X10.0" or "Y5.0").</returns>
        public string XYZEMoveString(MovePositions position)
        {
            switch (position)
            {
                case MovePositions.XMovePos:
                    if (XMovePosition != null)
                    {
                        return $"X{XMovePosition}";
                    }
                    return null;
                case MovePositions.YMovePos:
                    if (YMovePosition != null)
                    {
                        return $"Y{YMovePosition}";
                    }
                    return null;
                case MovePositions.ZMovePos:
                    if (ZMovePosition != null)
                    {
                        return $"Z{ZMovePosition}";
                    }
                    return null;
                case MovePositions.EMovePos:
                    if (EMovePosition != null)
                    {
                        return $"E{EMovePosition}";
                    }
                    return null;
            }
            return null;
        }
    }
}