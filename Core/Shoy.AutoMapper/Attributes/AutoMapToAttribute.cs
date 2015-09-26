using System;

namespace Shoy.AutoMapper.Attributes
{
    /// <summary> 映射到以下类型 </summary>
    public class AutoMapToAttribute : AutoMapAttribute
    {
        internal override AutoMapDirection Direction
        {
            get { return AutoMapDirection.To; }
        }

        public AutoMapToAttribute(params Type[] targetTypes)
            : base(AutoMapDirection.To, targetTypes)
        {

        }
    }
}
