namespace ControllingAndManagingApp.Motion
{
    public class MotionSettingsData
    {
        public int? XMaxFeedrate;
        public int? YMaxFeedrate;
        public int? ZMaxFeedrate;
        public int? EMaxFeedrate;

        public int? XDefaultFeedrate;
        public int? YDefaultFeedrate;
        public int? ZDefaultFeedrate;
        public int? EDefaultFeedrate;

        public int? XMaxAcceleration;
        public int? YMaxAcceleration;
        public int? ZMaxAcceleration;
        public int? EMaxAcceleration;

        public int? XDefaultAcceleration;
        public int? YDefaultAcceleration;
        public int? ZDefaultAcceleration;
        public int EDefaultAcceleration;

        public int StepsPerUnit;
        public int? FeedRateFreeMove;
        public double PrintSpeed;
        public int FanSpeed;
        public int PrintFlow;

        public double? XHomeOffset;
        public double? YHomeOffset;
        public double? ZHomeOffset;

        public double? XHomePos;
        public double? YHomePos;
        public double? ZHomePos;

        public string FeedRateString()
        {
            if (FeedRateFreeMove != null)
            {
                return $"F{FeedRateFreeMove}";
            }
            return null;
        }

        // Turn to string int fields
        public string XString(int? x)
        {
            if (x != null)
            {
                return $"X{x}";
            }
            return null;
        }
        public string YString(int? y)
        {
            if (y != null)
            {
                return $"Y{y}";
            }
            return null;
        }
        public string ZString(int? z)
        {
            if (z != null)
            {
                return $"Z{z}";
            }
            return null;
        }
        public string EString(int? e)
        {
            if (e != null)
            {
                return $"E{e}";
            }
            return null;
        }

        // Turn to string double fields
        public string XString(double? x)
        {
            if (x != null)
            {
                return $"X{x}";
            }
            return null;
        }
        public string YString(double? y)
        {
            if (y != null)
            {
                return $"Y{y}";
            }
            return null;
        }
        public string ZString(double? z)
        {
            if (z != null)
            {
                return $"Z{z}";
            }
            return null;
        }
    }
}