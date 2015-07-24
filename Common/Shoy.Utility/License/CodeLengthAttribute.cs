using System;

namespace Shoy.Utility.License
{
    /// <summary> 编码/激活码长度属性 </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CodeLengthAttribute : Attribute
    {
        public int Length { get; set; }

        public CodeLengthAttribute(int length)
        {
            Length = length;
        }
    }
}
