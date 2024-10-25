using System.Text.RegularExpressions;
using MachineControlHub;
using MachineControlHub.Motion;
using MudBlazor;
using static MachineControlHub.Print.CurrentPrintJob;

namespace WebUI.Data
{
    public class PrintingService
    {
        public const int MAX_FILE_SIZE = (1024 * 1024 * 90);

        public BackgroundTimer Background { get; set; }
        public PrinterDataService PrinterDataServiceTest { get; set; }

        // List of available drives with their names and volume labels
        public List<(string DriveName, string VolumeLabel)> DriversAvailable { get; set; } = new List<(string DriveName, string VolumeLabel)>();

        // List of files on the SD card with their names and sizes
        public List<(string FileName, string FileSize)> SDFiles { get; set; }

        // Static list to store hotend temperature graph data
        public static List<double> HotendGraph { get; set; } = new() { };

        // Static list to store bed temperature graph data
        public static List<double> BedGraph { get; set; } = new() { };

        // The drive letter of the currently selected drive
        public string DriveLetter { get; set; } = null;

        // Content of the uploaded file
        public string UploadedFileContent { get; set; }

        // Name of the file to be printed
        public string FileToPrint { get; set; } = "";

        // Options for the temperature chart
        public ChartOptions Options { get; set; } = new ChartOptions();

        // Labels for the X-axis of the temperature chart
        public string[] XAxisLabels { get; set; } = Array.Empty<string>();

        // Index for tracking purposes
        public int Index { get; set; } = -1;

        // Used to enable or disable start button on UI
        public bool StartButtonEnableDisable { get; set; } = true;

        // Used to enable or disable pause, resume and stop buttons on UI
        public bool PrintJobControlsEnableDisable { get; set; } = true;



        public PrintingService(PrinterDataService printerDataServiceTest)
        {
            PrinterDataServiceTest = printerDataServiceTest;
        }

        public void StartPrint(string fileName, Printer printer)
        {
            printer.PrintService.StartPrint(fileName);

            // Clears the estimation from the previous print if any
            printer.CurrentPrintJob.PrintProgressRecords.Clear();
        }

        public void PausePrint(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildPauseSDPrintCommand());
        }

        public void ResumePrint(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildStartSDPrintCommand());
        }

        public void CancelCurrentObject(Printer printer)
        {
            // Send the command to cancel the current object being printed
            printer.SerialConnection.Write("M486 C");
        }

        public void ListSDFiles(string inputText, Printer printer)
        {
            // Check if the printer's firmware supports long filenames
            if (printer.HasLongFilenameSupport)
            {
                SDFiles = printer.PrintService.ListLongNameSDFiles(inputText);
                printer.MediaAttached = true;
            }

            // If the printer's firmware does not support long filenames, use the standard method
            else
            {
                SDFiles = printer.PrintService.ListSDFiles(inputText);
                printer.MediaAttached = true;
            }
        }

        public void StartTimeOfPrint(Printer printer)
        {
            // store the start time of the print job
            printer.CurrentPrintJob.ParseStartTimeOfPrint();
        }

        /// <summary>
        /// Formats the total print time of the current print job.
        /// </summary>
        /// <param name="printer">The printer object containing the current print job.</param>
        public void FormatTotalPrintTime(Printer printer)
        {
            // Calculate the elapsed time from the print job's stopwatch
            TimeSpan elapsed = TimeSpan.FromMilliseconds(printer.CurrentPrintJob.Stopwatch.ElapsedMilliseconds);

            // Format the elapsed time as a string in the format "HH:mm:ss"
            printer.CurrentPrintJob.TotalPrintTime = string.Format($"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}");
        }

        /// <summary>
        /// Retrieves the file name and size from the list of SD files and updates the current print job.
        /// </summary>
        /// <param name="input">The name of the file to search for.</param>
        /// <param name="printer">The printer object containing the current print job.</param>
        public void GetFileNameAndSize(string input, Printer printer)
        {
            // Find the file in the list of SD files
            var file = SDFiles.FirstOrDefault(f => f.FileName == input);

            // If the file is found, update the current print job with the file name and size
            if (file != default)
            {
                printer.CurrentPrintJob.FileName = file.FileName;
                printer.CurrentPrintJob.FileSize = double.Parse(file.FileSize);
            }
        }

        /// <summary>
        /// List of chart series for temperature data.
        /// </summary>
        public List<ChartSeries> Series = new()
    {
        new() { Name = "Hotend", Data = HotendGraph.ToArray() },
        new() { Name = "Bed", Data = BedGraph.ToArray() },
    };

        /// <summary>
        /// Updates the graph data for the temperature chart.
        /// </summary>
        public void UpdateGraphData()
        {
            // Create a new list of chart series with updated data
            var new_series = new List<ChartSeries>()
    {
        new() { Name = "Hotend", Data = HotendGraph.ToArray() },
        new() { Name = "Bed", Data = BedGraph.ToArray() },
    };

            // Assign the new series to the Series property
            Series = new_series;
        }

        /// <summary>
        /// Calculates the percentage of the numerator with respect to the denominator.
        /// </summary>
        /// <param name="numerator">The numerator value.</param>
        /// <param name="denominator">The denominator value.</param>
        /// <returns>The calculated percentage as a double.</returns>
        public double CalculatePercentage(double numerator, double denominator)
        {
            double fraction = numerator / denominator;
            return fraction * 100;
        }

        /// <summary>
        /// Updates the print progress based on the provided input from the printer.
        /// </summary>
        /// <param name="input">The input received from the printer.</param>
        /// <param name="printer">The printer object containing the current print job.</param>
        public void UpdatePrintProgress(string input, Printer printer)
        {
            // Match the input string to check if it indicates printing progress
            var printing = Regex.Match(input, @"printing byte (\d+)/(\d+)");

            // Match the input string to check if it indicates the print job is finished
            var finished = Regex.Match(input, @"Done printing file");

            // Match the input string to check if it indicates the printer is not printing
            var notPrinting = Regex.Match(input.Trim(), @"\s*Not\s+SD\s*", RegexOptions.IgnoreCase);

            // If the input indicates printing progress
            if (printing.Success)
            {
                // Update the current print job's byte counts
                printer.CurrentPrintJob.CurrentBytes = int.Parse(printing.Groups[1].Value);
                printer.CurrentPrintJob.TotalBytes = int.Parse(printing.Groups[2].Value);
                printer.CurrentPrintJob.FileSize = printer.CurrentPrintJob.TotalBytes;

                // Add a new print progress record with the current byte count and timestamp
                printer.CurrentPrintJob.PrintProgressRecords.Add(new PrintProgressRecord { BytesPrinted = printer.CurrentPrintJob.CurrentBytes, Timestamp = DateTime.Now });

                // Keep only the last 100 print progress records
                if (printer.CurrentPrintJob.PrintProgressRecords.Count > 100)
                {
                    printer.CurrentPrintJob.PrintProgressRecords.RemoveAt(0);
                }

                // Calculate and update the print progress percentage
                printer.CurrentPrintJob.PrintProgress = Math.Round(CalculatePercentage(printer.CurrentPrintJob.CurrentBytes, printer.CurrentPrintJob.TotalBytes));
                printer.CurrentPrintJob.IsPrinting = true;
            }

            // If the input indicates the printer is not printing
            if (notPrinting.Success)
            {
                printer.CurrentPrintJob.IsPrinting = false;
                printer.CurrentPrintJob.PrintProgress = 0;
            }

            // If the input indicates the print job is finished
            if (finished.Success)
            {
                printer.CurrentPrintJob.PrintProgress = 100;
                printer.CurrentPrintJob.EndTimeOfPrint = DateTime.Now;
            }

            // If there is any print progress
            if (printer.CurrentPrintJob.PrintProgress > 0)
            {
                // If the finalization has not been executed, set the printer to printing state
                if (!printer.CurrentPrintJob.FinalizationExecuted)
                {
                    printer.CurrentPrintJob.IsPrinting = true;
                }

                // If the print progress is 100% and finalization has not been executed
                if (printer.CurrentPrintJob.PrintProgress == 100 && !printer.CurrentPrintJob.FinalizationExecuted)
                {
                    printer.CurrentPrintJob.IsPrinting = false;
                    printer.CurrentPrintJob.StopStopwatch();
                    printer.CurrentPrintJob.PrintProgress = 0;
                    FormatTotalPrintTime(printer);
                    PrinterDataServiceTest.AddPrintJobToHistory(printer);
                    printer.CurrentPrintJob.FinalizationExecuted = true;
                }
            }
        }

        /// <summary>
        /// Extracts and sets the current printing file name from the provided input.
        /// </summary>
        public void GetPrintingFileName(string fileName, Printer printer)
        {
            var printFile = Regex.Match(fileName, @"Current file: (.*)");

            try
            {
                if (printFile.Success)
                {
                    printer.CurrentPrintJob.FileName = printFile.Groups[1].Value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Writes the uploaded file content to the specified drive.
        /// </summary>
        public async Task WriteFileToDrive(string driveName, string fileName, Printer printer)
        {
            printer.IsTransferringFile = true;

            // Combine the drive name and file name to get the full file path
            string filePath = Path.Combine(driveName, fileName);

            await Task.Run(() => System.IO.File.WriteAllText(filePath, UploadedFileContent));

            printer.IsTransferringFile = false;
        }

        /// <summary>
        /// Retrieves the list of available removable drives and updates the DriversAvailable property.
        /// </summary>
        public void GetDrives()
        {
            // Clear the existing list of available drives
            DriversAvailable.Clear();

            // Iterate through all drives on the system
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                // Check if the drive is ready and is of type removable
                if (drive.IsReady && drive.DriveType == DriveType.Removable)
                {
                    // Add the drive name and volume label to the list of available drives
                    DriversAvailable.Add((drive.Name, drive.VolumeLabel));
                }
            }
        }

        /// <summary>
        /// Releases the media from the printer and clears the drive letter and SD files list.
        /// </summary>
        /// /// <param name="printer">The printer object to send the attach command to.</param>
        public void ReleaseMedia(Printer printer)
        {
            // Send the command to release the media
            printer.SerialConnection.Write("M22");

            // Clear the drive letter and SD files list
            DriveLetter = null;
            FileToPrint = null;
            SDFiles?.Clear();

            // Update the media attached status
            printer.MediaAttached = false;
        }

        /// <summary>
        /// Attaches the media to the printer and sets the media attached status.
        /// </summary>
        /// <param name="printer">The printer object to send the attach command to.</param>
        public void AttachMedia(Printer printer)
        {
            // Send the command to attach the media
            printer.SerialConnection.Write("M21");

            // Clear the drive letter
            DriveLetter = null;

            // Update the printer's media attached status
            printer.MediaAttached = true;
        }
    }
}