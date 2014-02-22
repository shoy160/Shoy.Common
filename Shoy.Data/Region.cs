using System;

namespace Shoy.Data
{
    [Serializable]
    public class Region
    {
        public int Start { get; set; }
        public int Size { get; set; }

        public Region(int index, int size)
        {
            Start = index*size;
            Size = size;
        }
    }
}
