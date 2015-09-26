
using System;

namespace Shoy.AutoMapper
{
    /// <summary> AutoMapperDirection </summary>
    [Flags]
    public enum AutoMapDirection
    {
        /// <summary> 从类型映射 </summary>
        From = 1,
        /// <summary> 映射到类型 </summary>
        To = 2
    }
}
