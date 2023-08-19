namespace ControllingAndManagingApp.Material
{
    public class PreheatingProfiles
    {
        public int? HotendTemp;
        public int? BedTemp;
        public int? FanSpeed;
        public int? MaterialIndex;


        public string HotendTempString()
        {
            if (HotendTemp != null)
            {
                return $"H{HotendTemp}";
            }
            return null;
        }

        public string BedTempString()
        {
            if (BedTemp != null)
            {
                return $"B{BedTemp}";
            }
            return null;
        }
        public string FanSpeedString()
        {
            if (FanSpeed != null)
            {
                return $"F{FanSpeed}";
            }
            return null;
        }

        public string MaterialIndexString()
        {
            if (MaterialIndex != null)
            {
                return $"S{MaterialIndex}";
            }
            return null;
        }

    }

}