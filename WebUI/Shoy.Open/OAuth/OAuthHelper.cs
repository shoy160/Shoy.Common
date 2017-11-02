using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Shoy.Open.OAuth
{
    /// <summary> 认证类型 </summary>
    public enum GrantType
    {
        /// <summary> client_credentials </summary>
        [Description("client_credentials")]
        ClientCredentials,

        /// <summary> password模式 </summary>
        [Description("password")]
        Password,

        /// <summary> authorization_code 模式 </summary>
        [Description("authorization_code")]
        AuthorizationCode,

        /// <summary> refresh_token 模式 </summary>
        [Description("refresh_token")]
        RefreshToken
    }

    public class TokenResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
    /// <summary> OAuth2辅助方法 </summary>
    public class OAuthHelper
    {
        private readonly string _host;
        private static readonly HttpClient Client = new HttpClient();

        public string TokenPath { get; set; }

        public string AuthorizePath { get; set; }

        /// <summary> OAuth2 构造函数 </summary>
        /// <param name="host"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public OAuthHelper(string host, string clientId = null, string clientSecret = null)
        {
            TokenPath = "/token";
            AuthorizePath = "/authorize";
            _host = host;
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));
        }

        /// <summary> password 模式 </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<TokenResult> Password(string username, string password)
        {
            var dict = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", username},
                {"password", password}
            };
            return await AccessToken(dict);
        }

        /// <summary> code 模式 </summary>
        /// <param name="code"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task<TokenResult> Code(string code, string redirectUri)
        {
            var dict = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", code},
                {"redirect_uri", redirectUri}
            };
            return await AccessToken(dict);
        }

        /// <summary> refresh_token 模式 </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<TokenResult> Refresh(string refreshToken)
        {
            var dict = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken}
            };
            return await AccessToken(dict);
        }

        /// <summary> 获取AccessToken </summary>
        /// <returns></returns>
        private async Task<TokenResult> AccessToken(IDictionary<string, string> dict)
        {
            var uri = new Uri(new Uri(_host), TokenPath);
            var content =
                new FormUrlEncodedContent(dict.Select(t => new KeyValuePair<string, string>(t.Key, t.Value)));
            var request = await Client.PostAsync(uri, content);
            var result = await request.Content.ReadAsAsync<TokenResult>();
            if (result == null)
                throw new Exception(await request.Content.ReadAsStringAsync());
            return result;
        }
    }
}