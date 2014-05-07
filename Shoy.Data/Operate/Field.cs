namespace Shoy.Data
{
    public class Field
    {
        public object Value { get; set; }

        private string _mParameterName;

        public string ParameterName
        {
            get { return string.IsNullOrEmpty(_mParameterName) ? Name : _mParameterName; }
            set { _mParameterName = value; }
        }

        public string Name { get; set; }

        private bool _mIsParameter = true;

        public bool IsParameter
        {
            get { return _mIsParameter; }
            set { _mIsParameter = value; }
        }

        public string GetValueBy { get; set; }
        public bool GetValueAfterInsert { get; set; }
    }
}
