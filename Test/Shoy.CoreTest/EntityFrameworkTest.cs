using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.CoreTest.Context;
using Shoy.CoreTest.Context.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Shoy.CoreTest
{
    [TestClass]
    public class EntityFrameworkTest //: TestBase
    {
        private readonly ITestDbRepository<User> _userRepository;
        public EntityFrameworkTest()
        {
            //_userRepository = Container.Resolve<ITestDbRepository<User>>();
        }

        [TestMethod]
        public void UserTest()
        {
            //var regist = Container.IsRegistered<ITestDbRepository<User>>();
            //Console.WriteLine(regist);
            //var user = _userRepository.Load(1);
            //Console.WriteLine(user.ToJson());

            //Task.Factory.StartNew(() =>
            //{
            //    var userRepository = Container.Resolve<ITestDbRepository<User>>();
            //    user = userRepository.Load(1);
            //    Console.WriteLine(user.ToJson());
            //}).Wait();
        }

        [TestMethod]
        public async Task Test_01()
        {
            const string uri = "https://auto.sinosafe.com.cn/DoubleCodeinput/query?_t={0}";
            const string referer = "https://auto.sinosafe.com.cn/";
            const string cookie =
                "aliyungf_tc=AQAAAPGZzQyavQwAWsXWqwl1FQYwVSBH; corevins80=At2tZwsEAgr0sPgKPUMjRg$$; JSESSIONID=fY_benM9P1Eici6X74D-qh8sjuj9O8aKjopY1heAkWF6XTYznpfh!-83576508; menu_cookie=%2Fquotation%2Fview";
            var url = string.Format(uri, DateTime.Now.Ticks);

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("precondition","companyCode,businessMode,,agreementNo"),
                new KeyValuePair<string, string>("preconditionValue","0110060101,2,,B0110100106"),
                new KeyValuePair<string, string>("manySelect","1"),
                new KeyValuePair<string, string>("hideValue",""),
                new KeyValuePair<string, string>("otherCondition",""),
                new KeyValuePair<string, string>("rowsPerPage","5"),
                new KeyValuePair<string, string>("pageNo","0"),
                new KeyValuePair<string, string>("fieldValue",""),
                new KeyValuePair<string, string>("codeType","getSalesmanCode")
            });

            //var cookieContainer = new CookieContainer();
            //cookieContainer.Add(new Cookie("aliyungf_tc", "AQAAAPGZzQyavQwAWsXWqwl1FQYwVSBH") { Domain = "auto.sinosafe.com.cn" });
            //cookieContainer.Add(new Cookie("corevins80", "At2tZwsEAgr0sPgKPUMjRg$$") { Domain = "auto.sinosafe.com.cn" });
            //cookieContainer.Add(new Cookie("JSESSIONID", "fY_benM9P1Eici6X74D-qh8sjuj9O8aKjopY1heAkWF6XTYznpfh!-83576508") { Domain = "auto.sinosafe.com.cn" });
            //var httpClientHandler = new HttpClientHandler
            //{
            //    CookieContainer = cookieContainer,
            //    AllowAutoRedirect = true,
            //    UseCookies = true
            //};

            var client = new HttpClient(new HttpClientHandler { UseCookies = false });

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Clear();
            req.Headers.Add("Referer", referer);
            req.Headers.Add("Cookie", cookie);
            //req.Headers.Add("Cookie", "aliyungf_tc=AQAAAPGZzQyavQwAWsXWqwl1FQYwVSBH");
            //req.Headers.Add("Cookie", "corevins80=At2tZwsEAgr0sPgKPUMjRg$$");
            //req.Headers.Add("Cookie", "JSESSIONID=fY_benM9P1Eici6X74D-qh8sjuj9O8aKjopY1heAkWF6XTYznpfh!-83576508");
            req.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            req.Content = data;
            var resp = await client.SendAsync(req, CancellationToken.None);
            var html = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(html);

            //const string paras = "precondition=companyCode%2CbusinessMode%2C%2CagreementNo&preconditionValue=0110060101%2C2%2C%2CB0110100106&manySelect=1&hideValue=&otherCondition=&rowsPerPage=5&pageNo=0&fieldValue=&codeType=getSalesmanCode";
            //using (var helper =
            //    new HttpHelper(url, "post", Encoding.UTF8, cookie, referer, paras))
            //{
            //    var html1 = helper.GetHtml();
            //    Console.Write(html1);
            //}
        }
    }
}
