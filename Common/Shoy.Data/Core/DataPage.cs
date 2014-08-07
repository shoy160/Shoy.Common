using System;

namespace Shoy.Data.Core
{
    public interface IDataPage
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }
        int RecordCount { get; set; }
        int PageCount { get; }
        string OrderField { get; set; }
    }

    public interface IDataPageProperty
    {
        IDataPage DataPage { get; set; }
    }

    [Serializable]
    public class DataPage : IDataPage
    {
        private int _pageIndex;

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        private int _pageSize = 10;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        private int _recordCount;

        public int RecordCount
        {
            get { return _recordCount; }
            set { _recordCount = value; }
        }

        public int PageCount
        {
            get
            {
                int mCount;
                if (PageSize == 0)
                    PageSize = 10;
                if (RecordCount%PageSize > 0)
                    mCount = RecordCount/PageSize + 1;
                else
                    mCount = RecordCount/PageSize;
                if (mCount == 0)
                    mCount = mCount + 1;
                return mCount;
            }
        }

        private string _orderField;

        public string OrderField
        {
            get { return _orderField; }
            set { _orderField = value; }
        }
    }
}
