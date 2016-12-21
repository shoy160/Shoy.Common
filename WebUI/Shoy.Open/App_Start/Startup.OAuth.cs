using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Shoy.Open.OAuth;
using System;

namespace Shoy.Open
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var oAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                AuthenticationMode = AuthenticationMode.Active,
                TokenEndpointPath = new PathString("/token"), //获取 access_token 认证服务请求地址
                AuthorizeEndpointPath = new PathString("/authorize"), //获取 authorization_code 认证服务请求地址
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(2), //access_token 过期时间

                Provider = new OpenAuthorizationServerProvider(), //access_token 相关认证服务
                AuthorizationCodeProvider = new OpenAuthorizationCodeProvider(), //authorization_code 认证服务
                RefreshTokenProvider = new OpenRefreshTokenProvider() //refresh_token 认证服务
            };
            app.UseOAuthAuthorizationServer(oAuthOptions); //表示 token_type 使用 bearer 方式
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }


}