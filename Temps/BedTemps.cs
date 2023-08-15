namespace ControllingAndManagingApp.Temps
{
    public class BedTemps
    {
        public PIDValues PIDBedValues; //TODO: introduce pidvalues class or have 3 variables here
        public int BedCurrentTemp;
        public int BedMaxTemp;
        public int BedSetTemp;
    }
}