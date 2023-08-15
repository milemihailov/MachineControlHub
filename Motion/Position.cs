namespace ControllingAndManagingApp.Motion
{
    public enum HomePositions
    {
        XHomePos,
        YHomePos,
        ZHomePos,
    }

    public enum MovePositions
    {
        XMovePos,
        YMovePos,
        ZMovePos,
        EMovePos
    }
    public class Position
    {
        public double XCurrentPosition;
        public double YCurrentPosition;
        public double ZCurrentPosition;
        public double ECurrentPosition;

        public double? XMovePosition;
        public double? YMovePosition;
        public double? ZMovePosition;
        public double? EMovePosition;

        public double? XHomePosition;
        public double? YHomePosition;
        public double? ZHomePosition;

        public string XYZEMoveString(MovePositions position)
        {
            switch (position)
            {
                case MovePositions.XMovePos:
                    if (XMovePosition != null)
                    {
                        return $"X{XMovePosition}";
                    }
                    return null;
                case MovePositions.YMovePos:
                    if (YMovePosition != null)
                    {
                        return $"Y{YMovePosition}";
                    }
                    return null;
                case MovePositions.ZMovePos:
                    if (ZMovePosition != null)
                    {
                        return $"Z{ZMovePosition}";
                    }
                    return null;
                case MovePositions.EMovePos:
                    if (EMovePosition != null)
                    {
                        return $"E{EMovePosition}";
                    }
                    return null;
            }
            return null;
        }

        public string XYZHomeString(HomePositions position)
        {
            switch (position)
            {
                case HomePositions.XHomePos:
                    if (XHomePosition != null)
                    {
                        return $"X{XHomePosition}";
                    }
                    return null;
                case HomePositions.YHomePos:
                    if (YHomePosition != null)
                    {
                        return $"Y{YHomePosition}";
                    }
                    return null;
                case HomePositions.ZHomePos:
                    if (ZHomePosition != null)
                    {
                        return $"Z{ZHomePosition}";
                    }
                    return null;
            }
            return null;
        }
    }
}