using System;

namespace nfynt
{

    [Flags]
    public enum DownFingers
    {
        None = 0,
        Thumb = 1,
        Index = 2,
        Middle = 4,
        Ring = 8,
        Little = 16
    }

    public enum HandGesture : byte
    {
        Na,           // Not defined
        Fist,         // FlagValue = 0
        ThumbsUp,     // FlagValue = 1
        Point,        // FlagValue = 2
        MiddleFinger, // FlagValue = 4
        RingFinger,   // FlagValue = 8
        Pinky,        // FlagValue = 16
        Victory,      // FlagValue = 6
        Rock,         // FlagValue = 18
        Ok,           // FlagValue = 28
        Shoot,          // FlagValue = 3
        Cool,         // FlagValue = 19
        OpenHand      // FlagValue = 31
    }

    public class Gestures
    {
        public static HandGesture EstimateHandGesture( int downFingers )
        {
            if ( downFingers >= 32 )
            {
                Console.WriteLine( "You've an alien hand :)" );

                return HandGesture.Na;
            }

            downFingers = 31 - downFingers;

            switch (downFingers)
            {
                case 0:
                    return HandGesture.Fist;
                case 1:
                    return HandGesture.ThumbsUp;
                case 2:
                    return HandGesture.Point;
                case 4:
                    return HandGesture.MiddleFinger;
                case 8:
                    return HandGesture.RingFinger;
                case 16:
                    return HandGesture.Pinky;
                case 3:
                    return HandGesture.Shoot;
                case 6:
                    return HandGesture.Victory;
                case 18:
                    return HandGesture.Rock;
                case 28:
                    return HandGesture.Ok;
                case 19:
                    return HandGesture.Cool;
                case 31:
                    return HandGesture.OpenHand;
            }
            return HandGesture.Na;
        }
    }

}