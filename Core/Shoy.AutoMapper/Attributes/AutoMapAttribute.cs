using System;

namespace Shoy.AutoMapper.Attributes
{
    public class AutoMapAttribute : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        private readonly AutoMapDirection _direction;

        internal virtual AutoMapDirection Direction
        {
            get { return _direction; }
        }

        public AutoMapAttribute(params Type[] targetTypes) :
            this(AutoMapDirection.From | AutoMapDirection.To, targetTypes)
        {
        }

        public AutoMapAttribute(AutoMapDirection direction, params Type[] targetTypes)
        {
            _direction = direction;
            TargetTypes = targetTypes;
        }
    }
}
