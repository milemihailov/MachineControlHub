namespace ControllingAndManagingApp.Motion
{
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
    }
}