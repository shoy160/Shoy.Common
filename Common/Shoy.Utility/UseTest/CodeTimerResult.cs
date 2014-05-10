using System;
using System.Text;

namespace Shoy.Utility.UseTest
{
    /// <summary>
    /// 表示 <see cref="CodeTimer"/> 执行结果的类.
    /// </summary>
    public class CodeTimerResult
    {
        /// <summary>
        /// 初始化 <see cref="CodeTimer"/> 类的新实例.
        /// </summary>
        public CodeTimerResult()
        {
            GenerationList = new Int32[GC.MaxGeneration + 1];
        }

        /// <summary>
        /// 名称.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 运行时间.(ms)
        /// </summary>
        public Int64 TimeElapsed { get; set; }

        /// <summary>
        /// Cpu 时钟周期(ns).ToString('N0')
        /// </summary>
        public long CpuCycles { get; set; }

        /// <summary>
        /// GC 代数集合.
        /// </summary>
        public Int32[] GenerationList { get; set; }

        /// <summary>
        /// 线程的计数.
        /// </summary>
        public Int32 ThreadCount { get; set; }

        /// <summary>
        /// 重复的次数.
        /// </summary>
        public Int32 Iteration { get; set; }

        /// <summary>
        /// 模拟思考的时间.
        /// </summary>
        public Int32 MockThinkTime { get; set; }

        /// <summary>
        /// 执行成功计数.
        /// </summary>
        public Int32 SuccessCount { get; set; }

        /// <summary>
        /// 执行失败计数.
        /// </summary>
        public Int32 FailureCount { get; set; }

        /// <summary>
        /// 重置 <see cref="CodeTimer"/>.
        /// </summary>
        /// <returns>重置后的 <see cref="CodeTimer"/> 对象实例.</returns>
        public CodeTimerResult Reset()
        {
            Name = String.Empty;
            TimeElapsed = 0;
            CpuCycles = 0;
            GenerationList = new Int32[GC.MaxGeneration + 1];

            return this;
        }
				
		public override string ToString()
        {
            var msg = new StringBuilder();
            msg.AppendLine("测试名称：" + Name + Environment.NewLine);
            msg.AppendLine("重复的次数：" + Iteration + Environment.NewLine);
            msg.AppendLine("线程的计数：" + ThreadCount + Environment.NewLine);
            msg.AppendLine("模拟思考的时间：" + MockThinkTime + Environment.NewLine);
            msg.AppendLine("运行时间(ms)：" + TimeElapsed + Environment.NewLine);
            msg.AppendLine("Cpu 时钟周期(ns)：" + CpuCycles.ToString("N0") + Environment.NewLine);
            msg.AppendLine("执行成功计数：" + SuccessCount + Environment.NewLine);
            msg.AppendLine("执行失败计数：" + FailureCount + Environment.NewLine);
            return msg.ToString();
        }
    }
}
