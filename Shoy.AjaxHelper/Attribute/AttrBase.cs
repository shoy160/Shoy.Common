using System;
using Shoy.AjaxHelper.Model;

namespace Shoy.AjaxHelper
{
    public abstract class AttrBase : System.Attribute, IComparable
    {
        public HttpRequestInfo CurrentHttpRequest { get; set; }
        internal MethodPathInfo MethodPath { get; set; }

        public int Level { get; protected set; }

        public AttrBase()
        {
            Level = 0;
        }

        /// <summary>
        /// 验证方法
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValidate()
        {
            if (CurrentHttpRequest.Context == null)
            {
                return false;
            }
            return true;
        }

        public int CompareTo(object obj)
        {
            var attr = obj as AttrBase;
            if (attr != null)
                return attr.Level.CompareTo(Level);
            throw new AjaxException("obj is not AttrBase");
        }
    }
}
