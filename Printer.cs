namespace ControllingAndManagingApp
{
    public class Printer
    {
        public string Model;
        public string PrinterFirmwareVersion;
        public PrinterBed Bed;
        public PrinterHead Head;
        public BedTemps BedTemperatures;
        public HotendTemps HotendTemperatures;
        public ChamberTemps ChamberTemperatures;
        public FilamentProperties FilamentUsed;
        public TouchScreen TouchScreen;
        public double MaxLayerHeight;
        public bool HasAutoBedLevel;
        public bool HasFilamentRunoutSensor;
        public bool NetworkConnection;
        public int ZMaxBuildVolume;

    }
}