using System;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Utility;
using Shoy.Utility.UseTest;
using Shoy.Utility.Extend;

namespace Shoy.Test
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void HttpHelperTest()
        {
            using (var http = new HttpHelper("www.baidu.com",Encoding.UTF8))
            {
                var html = http.GetHtml();
                Console.Write(html);
            }
        }

        [TestMethod]
        public void ExtTest()
        {
            var obj = new {state = 0, msg = "ddddd", data = new {test = "111", tds = "dsss"}};
            var timer01 = CodeTimer.Time("normal", 20*10000, () =>
                {
                    var str = new JavaScriptSerializer().Serialize(obj);
                });
            var timer02 = CodeTimer.Time("ext", 20*10000, () =>
                {
                    string str = obj.ToJson();
                });

            Console.WriteLine(timer01.ToString());
            Console.WriteLine(timer02.ToString());
        }
    }
}
