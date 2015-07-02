
namespace Shoy.Core.Data
{
    public class PageCondition
    {
        public int Index { get; set; }
        public int Size { get; set; }
        public SortCondition[] Conditions { get; set; }

        public PageCondition()
        {
            Index = 1;
            Size = 20;
            Conditions = new SortCondition[] { };
        }

        public PageCondition(int index, int size)
        {
            Index = index;
            Size = size;
            Conditions = new SortCondition[] {};
        }
    }
}
