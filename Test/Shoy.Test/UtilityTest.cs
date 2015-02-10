using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Utility;
using Shoy.Utility.Extend;
using Shoy.Utility.UseTest;

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
                http.AddFiles(new List<string> {"d:\\big.mp4"});
                var html = http.GetHtml();
                Console.Write(html);
            }
        }

        [TestMethod]
        public void ExtTest()
        {
            var obj = new {state = 0, msg = "ddddd", data = new {test = "111", tds = "dsss"}};
            var timer01 = CodeTimer.Time("normal", 20*10000,
                () => { var str = new JavaScriptSerializer().Serialize(obj); });
            var timer02 = CodeTimer.Time("ext", 20*10000, () => { var str = obj.ToJson(); });

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
            });
            Console.WriteLine(result.ToString());
            result = CodeTimer.Time("CombHelper", iteration, () =>
            {
                var guid = CombHelper.NewComb();
                Console.WriteLine(guid);
            });
            Console.WriteLine(result.ToString());
        }
    }
}
