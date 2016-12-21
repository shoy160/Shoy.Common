using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Utility.Helper;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Shoy.Test
{
    [TestClass]
    public class OAuthTest
    {
        private const string HostAddress = "http://localhost:13905";
        private const string RedirectUri = HostAddress + "/code";
        private const string ClientId = "shoy";
        private const string ClientSecret = "123456";
        private readonly OAuthHelper _helper;

        public OAuthTest()
        {
            _helper = new OAuthHelper(HostAddress, ClientId, ClientSecret);
        }

        [TestMethod]
        public async Task OAuth_ClientCredentials_Test()
        {
            var tokenResponse = _helper.GetTokenUseClientCredentials().Result;//GetToken("client_credentials").Result; //获取 access_token
            _helper.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _helper.Client.GetAsync("/users");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).Message);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Thread.Sleep(10000);

            var tokenResponseTwo = _helper.GetTokenUseRefreshToken(tokenResponse.RefreshToken).Result; //GetToken("refresh_token", tokenResponse.RefreshToken).Result;
            _helper.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseTwo.AccessToken);
            var responseTwo = await _helper.Client.GetAsync("/users");
            Assert.AreEqual(HttpStatusCode.OK, responseTwo.StatusCode);
        }

        [TestMethod]
        public async Task OAuth_Password_Test()
        {
            var tokenResponse = _helper.GetTokenUsePassword(ClientId, ClientSecret).Result; //GetToken("password", null, ClientId, ClientSecret).Result; //获取 access_token
            if (tokenResponse == null) return;
            _helper.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _helper.Client.GetAsync("/users");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).Message);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Thread.Sleep(10000);

            var tokenResponseTwo = _helper.GetTokenUseRefreshToken(tokenResponse.RefreshToken).Result; //GetToken("refresh_token", tokenResponse.RefreshToken).Result;
            _helper.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseTwo.AccessToken);
            var responseTwo = await _helper.Client.GetAsync("/users");
            Assert.AreEqual(HttpStatusCode.OK, responseTwo.StatusCode);
        }

        [TestMethod]
        public async Task OAuth_AuthorizationCode_Test()
        {
            //var authorizationCode = GetAuthorizationCode().Result; //获取 authorization_code
            var tokenResponse = _helper.GetTokenUseCode(RedirectUri).Result; //GetToken("authorization_code", null, null, null, authorizationCode).Result; //根据 authorization_code 获取 access_token
            _helper.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _helper.Client.GetAsync("/users");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).Message);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Thread.Sleep(10000);

            var tokenResponseTwo = _helper.GetTokenUseRefreshToken(tokenResponse.RefreshToken).Result; //GetToken("refresh_token", tokenResponse.RefreshToken).Result; //根据 refresh_token 获取 access_token
            _helper.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseTwo.AccessToken);
            var responseTwo = await _helper.Client.GetAsync("/users");
            Assert.AreEqual(HttpStatusCode.OK, responseTwo.StatusCode);
        }

        [TestMethod]
        public async Task OAuth_Implicit_Test()
        {
            //var tokenResponse = await _httpClient.GetAsync($"/authorize?response_type=token&client_id={ClientId}&redirect_uri={HttpUtility.UrlEncode(RedirectUri)}");
            //redirect_uri: http://localhost:8001/api/access_token#access_token=AQAAANCMnd8BFdERjHoAwE_Cl-sBAAAAfoPB4HZ0PUe-X6h0UUs2q42&token_type=bearer&expires_in=10
            //http://localhost:13905/code#access_token=EIqU0RO7gIoHbhj_cyzwrPjBbAVmFxzBvALRb5DtiGPMM0vqm5GkEYM8JW3w2QVc98-DhPHgLQaljSVf9DTO1cg2vvPXSdVk02L34KxThWQcGQPjm1KHRwIR-YqZZLTatKYTR9khW0YOpvIhD3nLYd0x3QYGu6h58efgtlkZUkO_zsAzZ25HGev-tT_oNJCKaYIGvAMdFQZTJlrFY0drcw&token_type=bearer&expires_in=7200
            var accessToken = "EIqU0RO7gIoHbhj_cyzwrPjBbAVmFxzBvALRb5DtiGPMM0vqm5GkEYM8JW3w2QVc98-DhPHgLQaljSVf9DTO1cg2vvPXSdVk02L34KxThWQcGQPjm1KHRwIR-YqZZLTatKYTR9khW0YOpvIhD3nLYd0x3QYGu6h58efgtlkZUkO_zsAzZ25HGev-tT_oNJCKaYIGvAMdFQZTJlrFY0drcw";//get form redirect_uri
            _helper.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _helper.Client.GetAsync("/users");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).Message);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
