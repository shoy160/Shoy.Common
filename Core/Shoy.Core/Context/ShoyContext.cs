using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;

namespace Shoy.Core.Context
{
    public class ShoyContext : Dictionary<string, object>
    {
        private const string CallContextKey = "__ShoyContext_CallContext_2eje9";
        private const string OperatorKey = "__ShoyContext_Operator_87dfh";
        public ShoyContext() { }

        /// <summary>
        /// 初始化一个<see cref="ShoyContext"/>类型的新实例
        /// </summary>
        protected ShoyContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// 获取或设置 当前上下文
        /// </summary>
        public static ShoyContext Current
        {
            get
            {
                var context = CallContext.LogicalGetData(CallContextKey) as ShoyContext;
                if (context != null)
                {
                    return context;
                }
                context = new ShoyContext();
                CallContext.LogicalSetData(CallContextKey, context);
                return context;
            }
            set
            {
                if (value == null)
                {
                    CallContext.FreeNamedDataSlot(CallContextKey);
                    return;
                }
                CallContext.LogicalSetData(CallContextKey, value);
            }
        }

        /// <summary>
        /// 获取 当前操作者
        /// </summary>
        public Operator Operator
        {
            get
            {
                if (!ContainsKey(OperatorKey))
                {
                    this[OperatorKey] = new Operator();
                }
                return this[OperatorKey] as Operator;
            }
        }
    }
}
