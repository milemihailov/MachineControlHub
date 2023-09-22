using ControllingAndManagingApp.Bed;
using ControllingAndManagingApp.Head;
using ControllingAndManagingApp.Material;
using ControllingAndManagingApp.Motion;
using ControllingAndManagingApp.Temps;

namespace ControllingAndManagingApp
{
    public class Printer
    {
        public string Model;
        public string PrinterFirmwareVersion;
        public bool HasAutoBedLevel;
        public bool HasFilamentRunoutSensor;
        public bool NetworkConnection;
        public int ZMaxBuildVolume;
        public PrinterBed Bed;
        public PrinterHead Head;
        public BedTemps BedTemperatures;
        public HotendTemps HotendTemperatures;
        public ChamberTemps ChamberTemperatures;
        public FilamentProperties FilamentUsed;
        public TouchScreen TouchScreen;
        public MotionSettingsData MotionSettings;
    }
}