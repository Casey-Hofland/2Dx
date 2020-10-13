using System;

namespace DimensionConverter
{
    [Flags]
    public enum OnConvertFlags
    {
        None    = 0,
        To2D    = 1 << 0,
        To3D    = 1 << 1,
    }
}
