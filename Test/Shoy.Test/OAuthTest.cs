using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Utility.Helper;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Shoy.Test
{
    [TestClass]
    public class OAuthTest
    {
        //private const string HostAddress = "http://localhost:13905";
        private const string HostAddress = "http://god.open.dayeasy.dev";
        private const string RedirectUri = HostAddress + "/code";
        //private const string ClientId = "shoy";
        //private const string ClientSecret = "123456";
        private const string ClientId = "god_android";
        private const string ClientSecret = "b712cf78c0410748";
        private readonly OAuthHelper _helper;

        public OAuthTest()
        {
            _helper = new OAuthHelper(HostAddress, ClientId, ClientSecret);
            _helper.TokenPath = "/oauth/token";
            _helper.AuthorizePath = "/oauth/authorize";
        }

        private void SetAccessToken(TokenResult token)
        {
            _helper.ClientHelper.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                token.AccessToken);
        }

        private async Task GetUsers(TokenResult token)
        {
            SetAccessToken(token);
            var response = await _helper.ClientHelper.GetAsync("/v1/user_load");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task OAuth_ClientCredentials_Test()
        {
            var tokenResponse = _helper.UseClientCredentials().Result;//获取 access_token
            await GetUsers(tokenResponse);

            Thread.Sleep(10000);

            var tokenResponseTwo = _helper.RefreshToken(tokenResponse.RefreshToken).Result;
            await GetUsers(tokenResponseTwo);

            //之前的还能不能用
            //await GetUsers(tokenResponse);
        }

        [TestMethod]
        public async Task OAuth_Password_Test()
        {

            var tokenResponse = _helper.UsePassword("18782246531", "123456").Result;//获取 access_token
            await GetUsers(tokenResponse);

            Thread.Sleep(10000);

            var tokenResponseTwo = _helper.RefreshToken(tokenResponse.RefreshToken).Result;
            await GetUsers(tokenResponseTwo);
        }

        [TestMethod]
        public async Task OAuth_AuthorizationCode_Test()
        {
            //var authorizationCode = GetAuthorizationCode().Result; //获取 authorization_code
            var tokenResponse = _helper.UseCode(RedirectUri).Result; //根据 authorization_code 获取 access_token
            await GetUsers(tokenResponse);

            Thread.Sleep(10000);

            var tokenResponseTwo = _helper.RefreshToken(tokenResponse.RefreshToken).Result;//根据 refresh_token 获取 access_token
            await GetUsers(tokenResponseTwo);
        }

        [TestMethod]
        public async Task OAuth_Implicit_Test()
        {
            //var tokenResponse = await _httpClient.GetAsync($"/authorize?response_type=token&client_id={ClientId}&redirect_uri={HttpUtility.UrlEncode(RedirectUri)}");
            //redirect_uri: http://localhost:8001/api/access_token#access_token=AQAAANCMnd8BFdERjHoAwE_Cl-sBAAAAfoPB4HZ0PUe-X6h0UUs2q42&token_type=bearer&expires_in=10
            //http://localhost:13905/code#access_token=EIqU0RO7gIoHbhj_cyzwrPjBbAVmFxzBvALRb5DtiGPMM0vqm5GkEYM8JW3w2QVc98-DhPHgLQaljSVf9DTO1cg2vvPXSdVk02L34KxThWQcGQPjm1KHRwIR-YqZZLTatKYTR9khW0YOpvIhD3nLYd0x3QYGu6h58efgtlkZUkO_zsAzZ25HGev-tT_oNJCKaYIGvAMdFQZTJlrFY0drcw&token_type=bearer&expires_in=7200
            var accessToken = "EIqU0RO7gIoHbhj_cyzwrPjBbAVmFxzBvALRb5DtiGPMM0vqm5GkEYM8JW3w2QVc98-DhPHgLQaljSVf9DTO1cg2vvPXSdVk02L34KxThWQcGQPjm1KHRwIR-YqZZLTatKYTR9khW0YOpvIhD3nLYd0x3QYGu6h58efgtlkZUkO_zsAzZ25HGev-tT_oNJCKaYIGvAMdFQZTJlrFY0drcw";//get form redirect_uri
            await GetUsers(new TokenResult { AccessToken = accessToken });
        }
    }
}
