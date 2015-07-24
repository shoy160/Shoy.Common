using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Utility;
using Shoy.Utility.Extend;
using Shoy.Utility.Observer;
using System;
using System.Threading;

namespace Shoy.Test
{
    /// <summary>
    /// ObserverTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ObserverTest
    {
        public class ObserverInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        /// <summary> 发布者 喵咪</summary>
        public class Cat : PublisherBase<ObserverInfo>
        {
            private readonly string _name;

            public Cat(string name)
            {
                _name = name;
            }

            public void Cry()
            {
                var info = new ObserverInfo
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = _name
                };
                Console.WriteLine(Utils.GetTimeNow());
                string action = "Cat {0}[{1}]: Cry..".FormatWith(info.Name, info.Id);
                Console.WriteLine(action);
                //Notify(info);
                NotifyAsync(info);
            }
        }

        /// <summary> 接收者 老鼠 </summary>
        public class Mouse : ObserverBase<ObserverInfo>
        {
            private readonly string _name;

            public Mouse(string name, params PublisherBase<ObserverInfo>[] publishers)
                : base(publishers)
            {
                _name = name;
            }

            protected override void Response(ObserverInfo sender)
            {
                Console.WriteLine(Utils.GetTimeNow());
                Console.WriteLine("由于{0}[{1}],老鼠{2}:赶快逃跑吧！~", sender.Name, sender.Id, _name);
            }

            public void RuningMouse()
            {
                Console.WriteLine("Mouse is Runing!");
            }
        }

        /// <summary>
        /// 接收者 宝宝
        /// </summary>
        public class Baby : ObserverBase<ObserverInfo>
        {
            private readonly string _name;

            public Baby(string name, params PublisherBase<ObserverInfo>[] publishers)
                : base(publishers)
            {
                _name = name;
            }

            protected override void Response(ObserverInfo sender)
            {
                Console.WriteLine(Utils.GetTimeNow());
                Console.WriteLine("由于{0}[{2}],宝宝{1}:被惊醒！", sender.Name, _name, sender.Id);
                Console.WriteLine("由于{0}[{2}],宝宝{1}:开始大哭！", sender.Name, _name, sender.Id);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var cat1 = new Cat("Cat01");
            var cat2 = new Cat("Cat02");
            var mouse = new Mouse("小老鼠", cat1, cat2);
            var baby = new Baby("Kimi", cat1, cat2);
            cat1.Update += sender => mouse.RuningMouse();
            cat1.Cry();
            Thread.Sleep(2500);
            cat2.Cry();
        }
    }
}
