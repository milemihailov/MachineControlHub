namespace MachineControlHub.Head
{
    /// <summary>
    /// Represents a camera device with various properties.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Gets or sets the model of the camera.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the width of the camera's resolution.
        /// </summary>
        public int ResolutionWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the camera's resolution.
        /// </summary>
        public int ResolutionHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the camera is currently recording.
        /// </summary>
        public bool IsRecording { get; set; }

        /// <summary>
        /// Gets or sets the focal length of the camera's lens.
        /// </summary>
        public double LensFocalLength { get; set; }
    }

}