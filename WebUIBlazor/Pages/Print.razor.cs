using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Plotly.Blazor.Interop;
using Plotly.Blazor;
using System.Globalization;
using Plotly.Blazor.Traces;

namespace WebUI.Pages
{
    public partial class Print
    {
        public string fileToPrint = "";
        public List<string> files;
        public string file;

        private int hours;
        private int minutes;
        private int seconds;
        private Timer timer;

        private async Task LoadFiles(InputFileChangeEventArgs e)
        {
            file = await new StreamReader(e.File.OpenReadStream(Data.PrintingService.MAX_FILE_SIZE)).ReadToEndAsync();
            print.ExtractPrintingSettings(file);
            StateHasChanged();
        }

        // private void StartClock()
        // {
        //     hours = 0;
        //     minutes = 0;
        //     seconds = 0;

        //     timer = new System.Timers.Timer(1000);
        //     timer.Elapsed += (sender, e) => UpdateClock();
        //     timer.AutoReset = true;
        //     timer.Enabled = true;
        // }

        private void UpdateClock()
        {
            seconds++;
            if (seconds == 60)
            {
                seconds = 0;
                minutes++;
                if (minutes == 2)
                {
                    print.EstimatedPrintTime();
                }
                if (minutes == 60)
                {
                    minutes = 0;
                    hours++;
                }
            }
            InvokeAsync(() => StateHasChanged());
        }

        [CascadingParameter]
        private MudTheme Theme { get; set; }

        private PlotlyChart chart;
        private Config config;
        private Layout layout;
        private IList<ITrace> data;

        private IEnumerable<EventDataPoint> ClickInfos { get; set; }
        private IEnumerable<EventDataPoint> HoverInfos { get; set; }

        private bool IsInitialized { get; set; }
        private bool _processing = false;

        private async Task Calibrate()
        {
            _processing = true;
            await Task.Run(() => levelBed.CalibrateBed());
            levelBed.meshData = GetSurfaceData();
            LoadData();
            _processing = false;
        }

        private void LoadData()
        {
            Task.Run(async () =>
            {
                foreach (var trace in await levelBed.meshData)
                {
                    await InvokeAsync(async () => await chart.AddTrace(trace));
                    await Task.Delay(100);
                }
                IsInitialized = true;
                await chart.SubscribeHoverEvent();
                await chart.SubscribeClickEvent();
                await InvokeAsync(StateHasChanged);
            });
        }

        public void ClickAction(IEnumerable<EventDataPoint> eventData)
        {
            ClickInfos = eventData;
            StateHasChanged();
        }

        public void HoverAction(IEnumerable<EventDataPoint> eventData)
        {
            HoverInfos = eventData;
            StateHasChanged();
        }

        async Task<IList<ITrace>> GetSurfaceData()
        {
            IList<ITrace> mapData = new List<ITrace>();

            using var client = new HttpClient
            {
                BaseAddress = new Uri(MyNavigationManager.BaseUri)
            };

            var csv = await Task.Run(() => levelBed.CSVData
                .Split("\n")
                .Skip(1)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Split(",").ToList()).ToList());

            var z = new List<decimal[]>();

            foreach (var row in csv)
            {
                var currentValues = new List<decimal>();

                for (var i = 1; i < row.Count; i++)
                {
                    currentValues.Add(decimal.Parse(row[i], NumberStyles.Any, CultureInfo.InvariantCulture));
                }

                z.Add(currentValues.ToArray());
            }

            mapData.Add(new Surface
            {
                Z = z.Cast<object>().ToList()
            });

            return mapData;
        }

        public static List<double> hote = new List<double> { };

        private async void UpdateTemperatureValues(object state)
        {
            if (serial.initialized)
            {
                hotend.ParseCurrentHotendTemperature();
                hote.Add(hotend.currentHotendTemperature);
                bed.ParseCurrentBedTemperature();

                await InvokeAsync(StateHasChanged);
            }
        }

        private int Index = -1; //default value cannot be 0 -> first selectedindex is 0.
        public ChartOptions Options = new ChartOptions();

        public List<ChartSeries> Series = new List<ChartSeries>()
    {
        new ChartSeries() { Name = "Hotend", Data = hote.ToArray() },
        new ChartSeries() { Name = "Bed", Data = new double[] { 10, 41, 35, 51, 49, 62, 69, 91, 148 } },
    };
        public string[] XAxisLabels = { };

        private void Confirm()
        {
            var parameters = new DialogParameters<Dialog>
            {
                { x => x.ContentText, "Start Print ?" },
                { x => x.ButtonText, "Yes" },
                { x => x.Color, Color.Success }
            };

            DialogService.Show<Dialog>("Confirm", parameters);

            print.StartPrint(fileToPrint);
            print.StartTimeOfPrint();
            print.GetFileNameAndSize(fileToPrint);
        }

        void IDisposable.Dispose()
        {
            timer?.Dispose();
        }
    }
}
