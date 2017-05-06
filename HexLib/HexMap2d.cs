using System.Collections.Generic;

namespace HexLib
{
    public class HexMap2d<TValue> : Dictionary<AxialPoint, TValue>
    {
        public TValue this[OffsetPoint key]
        {
            get { return this[key.ToAxial()]; }
            set { this[key.ToAxial()] = value; }
        }
    }
}
