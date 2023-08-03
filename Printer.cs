namespace ControllingAndManagingApp
{
    public class Printer
    {
        public string Model;
        public string PrinterFirmwareVersion;
        public PrinterBed Bed;
        public PrinterHead Head;
        public Camera CameraPresent;
        public BedTemps BedTemperatures;
        public HotendTemps HotendTemperatures;
        public ChamberTemps ChamberTemperatures;
        public FilamentProperties FilamentUsed;
        public int MaxLayerHeight;
        public bool AutoBedLevel;
        public bool NetworkConnection;
        public int ZMaxBuildVolume;
    }
}
