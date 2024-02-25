using MachineControlHub.Motion;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Plotly.Blazor.Interop;
using Plotly.Blazor;
using System.Text.RegularExpressions;

namespace WebUI.Pages
{
    public partial class ControlPanel
    {
        [CascadingParameter]
        private MudTheme Theme { get; set; }
        MudChip selected;
        private PlotlyChart chart;
        private Config config;
        private Layout layout;
        private IList<ITrace> data;
        private IEnumerable<EventDataPoint> ClickInfos { get; set; }
        private Timer timer;

        public int fanSpeed;
        public bool SwitchValue = false;
        private void ToggleValue()
        {
            if (SwitchValue)
                fanSpeed = 255;
            else
                fanSpeed = 0;
            SwitchValue = !SwitchValue;
        }

        private async void UpdateParagraph(object state)
        {
            if (serial.initialized)
            {
                Thread.Sleep(50);
                string message = serial.Read();

                double value;
                Match match = Regex.Match(message, Data.ControlPanelService.FAN_PATTERN);
                if (match.Success)
                {
                    value = double.Parse(match.Groups[1].Value);
                    control.CalculateFanSpeedIntoPercentage(value);
                }

                string filteredOutput = message.Replace("ok", " ");
                control.consoleOutput += filteredOutput;
                await InvokeAsync(StateHasChanged);
            }
        }

        public void ClickAction(IEnumerable<EventDataPoint> eventData)
        {
            ClickInfos = eventData;
            StoreClickedValues();
            StateHasChanged();
        }

        public async void SubscribeEvents()
        {
            await chart.SubscribeClickEvent();
        }

        MachineControlHub.Motion.Position pos = new();
        MotionSettingsData feedrate = new();

        private void StoreClickedValues()
        {
            if (ClickInfos != null && ClickInfos.Any())
            {
                string x = ClickInfos.First().X.ToString();
                string y = ClickInfos.FirstOrDefault(d => d.TraceIndex == 0)?.Y.ToString();
                pos.XMovePosition = double.Parse(x);
                pos.YMovePosition = double.Parse(y);
                serial.Write(CommandMethods.BuildAbsolutePositionCommand());
                serial.Write(CommandMethods.BuildLinearMoveCommand(pos, feedrate));
                serial.Write(CommandMethods.BuildRelativePositionCommand());
            }
        }

        void IDisposable.Dispose()
        {
            timer.Dispose();
        }
    }
}
