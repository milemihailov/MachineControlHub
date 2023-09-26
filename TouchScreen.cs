namespace ControllingAndManagingApp
{
    /// <summary>
    /// Represents the touchscreen configuration and properties of a 3D printer.
    /// </summary>
    public class TouchScreen
    {
        /// <summary>
        /// Gets or sets a value indicating whether the printer has a touchscreen.
        /// </summary>
        public bool HasTouchScreen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the touchscreen is enabled.
        /// </summary>
        public bool TouchScreenEnabled { get; set; }

        /// <summary>
        /// Gets or sets the width of the touchscreen screen in pixels.
        /// </summary>
        public int ScreenWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the touchscreen screen in pixels.
        /// </summary>
        public int ScreenHeight { get; set; }
    }

}