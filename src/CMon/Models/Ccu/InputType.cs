
namespace CMon.Models.Ccu
{
    public enum InputType : byte
    {
        Discrete = 0,
        Analog = 1,
        Rtd02 = 2,
        Rtd03 = 3,
        Rtd04 = 4,
        Rtd05 = 6 // ?
    }

    public enum RangeType : byte
    {
        None = 0,
        LowOrHigh = 1,
        Low = 2,
        Average = 3,
        High = 4,
        LowHysteresis = 5,
        HighHysteresis = 6
    }
}