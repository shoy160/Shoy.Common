using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Shoy.Core.Data
{
    /// <summary> 排序条件 </summary>
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

    public class SortCondition<T> : SortCondition
    {
        public SortCondition(Expression<Func<T, object>> keySelector)
            : this(keySelector, ListSortDirection.Ascending)
        {
        }

        public SortCondition(Expression<Func<T, object>> keySelector, ListSortDirection direction)
            : base(GetPropName(keySelector), direction)
        {
        }

        private static string GetPropName(Expression<Func<T, object>> keySelector)
        {
            string param = keySelector.Parameters.First().Name;
            string operand = (((dynamic) keySelector.Body).Operand).ToString();
            operand = operand.Substring(param.Length + 1, operand.Length - param.Length - 1);
            return operand;
        }
    }
}
