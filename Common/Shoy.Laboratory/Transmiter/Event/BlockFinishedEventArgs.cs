using System;

namespace Shoy.Laboratory.Transmiter
{
    public class BlockFinishedEventArgs : EventArgs
    {
        public int BlockIndex { get; set; }

        public BlockFinishedEventArgs(int blockIndex)
        {
            BlockIndex = blockIndex;
        }
    }
}
