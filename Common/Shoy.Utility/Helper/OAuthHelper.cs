using Shoy.Utility.Extend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Shoy.Utility.Helper
{
    public class TokenResult
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string TokenType { get; set; }

        public int ExpiresIn { get; set; }
    }

    public enum GrantType
    {
        [Description("client_credentials")]
        ClientCredentials,
        [Description("password")]
        Password,
        [Description("authorization_code")]
        AuthorizationCode,
        [Description("refresh_token")]
        RefreshToken
    }

    public class OAuthHelper
    {
        private string _clientId;
        private string _clientSecret;

        public string TokenPath { get; set; }
        public string AuthorizePath { get; set; }
        public HttpClient ClientHelper { get; }

        public OAuthHelper(string host, string clientId = null, string clientSecret = null)
        {
            TokenPath = "/token";
            AuthorizePath = "/authorize";
            ClientHelper = new HttpClient { BaseAddress = new Uri(host) };
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public void SetClient(string id, string secret)
        {
            _clientId = id;
            _clientSecret = secret;
        }

        private async Task<TokenResult> AccessTokenAsync(GrantType grantType, string refreshToken = null, string userName = null,
            string password = null, string authorizationCode = null, string redirectUri = null)
        {
            var parameters = new Dictionary<string, string> { { "grant_type", grantType.GetText() } };

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                parameters.Add("mobile", userName);
                parameters.Add("password", password);
            }
            if (!string.IsNullOrEmpty(authorizationCode))
            {
                parameters.Add("code", authorizationCode);
                parameters.Add("redirect_uri", redirectUri); //和获取 authorization_code 的 redirect_uri 必须一致，不然会报错
            }
            if (!string.IsNullOrEmpty(refreshToken))
            {
                parameters.Add("refresh_token", refreshToken);
            }

            ClientHelper.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(_clientId + ":" + _clientSecret)));

            var response = await ClientHelper.PostAsync(TokenPath, new FormUrlEncodedContent(parameters));
            var responseValue = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
                return JsonHelper.Json<TokenResult>(responseValue, NamingType.UrlCase);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(responseValue);
            return null;
        }

        private async Task<string> AuthorizationCodeAsync(string redirectUri)
        {
            var response =
                await
                    ClientHelper.GetAsync(
                        $"{AuthorizePath}?grant_type=authorization_code&response_type=code&client_id={_clientId}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
                return content.Trim('\"');
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(content);
            return null;
        }

        public async Task<TokenResult> UseClientCredentials()
        {
            return await AccessTokenAsync(GrantType.ClientCredentials);
        }

        public async Task<TokenResult> UsePassword(string account, string password)
        {
            return await AccessTokenAsync(GrantType.Password, userName: account, password: password);
        }

        public async Task<TokenResult> UseCode(string redirectUri)
        {
            var authorizationCode = AuthorizationCodeAsync(redirectUri).Result;
            if (string.IsNullOrWhiteSpace(authorizationCode))
                return null;
            return
                await
                    AccessTokenAsync(GrantType.AuthorizationCode, authorizationCode: authorizationCode, redirectUri: redirectUri);
        }

        public async Task<TokenResult> RefreshToken(string refreshToken)
        {
            return await AccessTokenAsync(GrantType.RefreshToken, refreshToken);
        }
    }
}
