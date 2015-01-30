using System.ComponentModel;

namespace Shoy.Core.Data
{
    public class SortCondition
    {
        public SortCondition(string sortField)
            : this(sortField, ListSortDirection.Ascending)
        {
        }

        public SortCondition(string sortField, ListSortDirection direction)
        {
            SortField = sortField;
            Sort = direction;
        }

        public string SortField { get; set; }

        public ListSortDirection Sort { get; set; }
    }
}
