using MachineControlHub.Bed;
using MachineControlHub.Motion;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using System.Globalization;
using WebUI.Pages;
using static MudBlazor.CategoryTypes;

namespace WebUI.Data
{
    public class BedLevelingService
    {
        public BedLevelData bedData;
        public string CSVData;
        public Task<IList<ITrace>> meshData;
        public bool _isInitialized { get; set; }



        public BedLevelingService() 
        {
            bedData = new BedLevelData();
        }

        public void CalibrateBed()
        {
            //ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildBedLevelingCommand());
            //ConnectionServiceSerial.printerConnection.CheckForBusy();
            //string input = ConnectionServiceSerial.printerConnection.Read();
            //CSVData = bedData.GetGrid(input);
        }

        public async Task<IList<ITrace>> GetSurfaceData()
        {
            IList<ITrace> mapData = new List<ITrace>();

            var csv = await Task.Run(() => CSVData
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
    }
}
