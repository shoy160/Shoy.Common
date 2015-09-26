using System;

namespace Shoy.AutoMapper.Attributes
{
    /// <summary> 从以下类型映射 </summary>
    public class AutoMapFromAttribute : AutoMapAttribute
    {
        public AutoMapFromAttribute(params Type[] targetTypes)
            : base(AutoMapDirection.From, targetTypes)
        {
        }
    }
}
