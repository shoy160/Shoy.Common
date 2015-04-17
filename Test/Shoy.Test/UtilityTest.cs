using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Laboratory;
using Shoy.Utility;
using Shoy.Utility.Extend;
using Shoy.Utility.UseTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Shoy.Test
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void HttpHelperTest()
        {
            using (var http = new HttpHelper("www.baidu.com", Encoding.UTF8))
            {
                var html = http.GetHtml();
                Console.Write(html);
            }
        }

        [TestMethod]
        public void PostFilesTest()
        {
            using (var http = new HttpHelper("http://file.dayeasy.dev/uploader?type=2", "post", Encoding.UTF8, ""))
            {
                http.AddFiles(new List<string> { "d:\\big.mp4" });
                var html = http.GetHtml();
                Console.Write(html);
            }
        }

        [TestMethod]
        public void ExtTest()
        {
            var obj = new { state = 0, msg = "ddddd", data = new { test = "111", tds = "dsss" } };
            var timer01 = CodeTimer.Time("normal", 20 * 10000,
                () => { var str = new JavaScriptSerializer().Serialize(obj); });
            var timer02 = CodeTimer.Time("ext", 20 * 10000, () => { var str = obj.ToJson(); });

            Console.WriteLine(timer01.ToString());
            Console.WriteLine(timer02.ToString());
        }

        [TestMethod]
        public void SpellTest()
        {
            const string str = "重庆";
            Console.WriteLine(Utils.GetSpellCode(str));
        }

        [TestMethod]
        public void JoinTest()
        {
            Console.WriteLine(new[]
            {
                new
                {
                    id = 1,
                    name = "test1"
                },
                new
                {
                    id = 2,
                    name = "test2"
                }
            }.Join(" and ", "[{id}] : [{name}]"));
        }

        [TestMethod]
        public void CombTest()
        {
            const int iteration = 50;
            var result = CodeTimer.Time("Guid", iteration, () =>
            {
                var guid = Guid.NewGuid();
                Console.WriteLine(guid);
            });
            Console.WriteLine(result.ToString());
            result = CodeTimer.Time("CombHelper", iteration, () =>
            {
                var guid = CombHelper.NewComb();
                Console.WriteLine(guid);
            });
            Console.WriteLine(result.ToString());
        }

        [TestMethod]
        public void DiscreMarkovTest()
        {
            //历史状态数据
            var list = new List<int>
            {
                8,15,2,15,16,3,7,9,12,14,1,6,10,2,3,6,8,
                14,12,5,8,12,16,2,4,6,7,14,12,1,13,4,6,9,8,2,12,14,5,13,16,1,7,6,4,3,
                6,7,14,9,2,8,10,15,10,11,12,14,14,8,16,8,2,5,14,11,7,16,9,
                6, 10, 13, 16, 14, 11, 1, 10, 13, 1,
                9, 8, 12, 6, 5, 15, 10, 15, 13, 8,
                3, 14, 9,15,6,1,11,4,13,12
            };
            var discrete = new DiscreteMarkov(list.ToList(), 16, 8);
            Console.WriteLine(discrete.PredictValue.ToJson());
            var dict = new Dictionary<int, double>();
            int value = 0;
            double max = 0;
            int index = 0;
            foreach (double v in discrete.PredictValue)
            {
                index++;
                dict.Add(index, v);
                if (!(v > max)) continue;
                max = v;
                value = index;
            }
            var preds = dict.OrderByDescending(t => t.Value).Select(t => new { num = t.Key, pred = t.Value });
            Console.WriteLine(preds.Join("\r\n", "{num}:{pred}"));
            Console.WriteLine("预测：{0}[{1}]", value, max);
            //Console.WriteLine("实际：{0}", random.Next(1, 16));
        }

        [TestMethod]
        public void SpeekerTest()
        {
            SpeekHelper.Speek("杨本国");
        }

        [TestMethod]
        public void ReaderTest()
        {
            const string name = "sim01.png";
            var reader = new ImageReader(null, LanguageType.Chinese);
            using (var bmp = (Bitmap)Image.FromFile(name))
            {
                var str = reader.Read(bmp, null, true);
                Console.WriteLine(str);
                //foreach (byte i in new byte[] { 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170 })
                //{
                //    Console.WriteLine(i);
                //    using (var item = ImageHelper.BinarizeImage(bmp.Clone() as Bitmap, i))
                //    {
                //        str = reader.Read(item, null, true);
                //        Console.WriteLine(str);
                //    }
                //}
            }
        }
    }
}
