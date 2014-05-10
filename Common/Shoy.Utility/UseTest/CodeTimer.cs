using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Shoy.Utility.UseTest
{
    public interface IAction
    {
        void Action();
    }

    public static class CodeTimer
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime,
                                          out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        public delegate void ActionDelegate();

        private static long GetCurrentThreadTimes()
        {
            long l;
            long kernelTime, userTimer;
            GetThreadTimes(GetCurrentThread(), out l, out l, out kernelTime,
                           out userTimer);
            return kernelTime + userTimer;
        }

        static CodeTimer()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

        }

        public static CodeTimerResult Time(string name, int iteration, ActionDelegate action)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            if (action == null)
            {
                return null;
            }

            var result = new CodeTimerResult();
            result = result.Reset();
            result.Name = name;
            result.Iteration = iteration;

            GC.Collect(GC.MaxGeneration);
            var gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            // 3. Run action
            var watch = new Stopwatch();
            watch.Start();
            long ticksFst = GetCurrentThreadTimes(); //100 nanosecond one tick

            for (int i = 0; i < iteration; i++) action();
            long ticks = GetCurrentThreadTimes() - ticksFst;
            watch.Stop();

            // 4. Print CPU
            result.TimeElapsed = watch.ElapsedMilliseconds;
            result.CpuCycles = ticks*100;

            // 5. Print GC
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                result.GenerationList[i] = count;
            }
            return result;
        }

        public static CodeTimerResult Time(string name, int iteration, IAction action)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            if (action == null)
            {
                return null;
            }

            var result = new CodeTimerResult();
            result = result.Reset();
            result.Name = name;
            result.Iteration = iteration;

            GC.Collect(GC.MaxGeneration);
            var gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            // 3. Run action
            var watch = new Stopwatch();
            watch.Start();
            long ticksFst = GetCurrentThreadTimes(); //100 nanosecond one tick

            for (int i = 0; i < iteration; i++) action.Action();
            long ticks = GetCurrentThreadTimes() - ticksFst;
            watch.Stop();

            // 4. Print CPU
            result.TimeElapsed = watch.ElapsedMilliseconds;
            result.CpuCycles = ticks*100;

            // 5. Print GC
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                result.GenerationList[i] = count;
            }
            return result;
        }

        public static string UseTest()
        {
            var result1 = Time("contact", 1000*200, () =>
                                                      {
                                                          string str = "";
                                                          for (int i = 0; i < 10; i++)
                                                          {
                                                              str += "dddddddddddddddddddddd";
                                                          }
                                                      });
            var result2 = Time("stringbuilder", 1000*200, () =>
                                                             {
                                                                 var sb = new System.Text.StringBuilder();
                                                                 for (int i = 0; i < 10; i++)
                                                                 {
                                                                     sb.Append("dddddddddddddddddddddd");
                                                                 }
                                                             });
            return result1.ToString() + result2.ToString();
        }
    }
}