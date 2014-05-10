using System;
using System.Data;

namespace Shoy.Data
{
    [Serializable]
    public class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        private ParameterDirection _direction = ParameterDirection.Input;

        public ParameterDirection Direction { get { return _direction; } set { _direction = value; } }
    }
}
