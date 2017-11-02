using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Shoy.Open.OAuth
{
    public class OpenAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 验证 client 信息
        /// </summary>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }
            if (clientId != "shoy" || clientSecret != "123456")
            {
                context.SetError("invalid_client", "client or clientSecret is not valid");
                return;
            }
            context.Validated();
            await base.ValidateClientAuthentication(context);
        }

        /// <summary> 生成 access_token（client credentials 授权方式） </summary>
        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var scopes = context.Scope.Select(x => new Claim("urn:oauth:scope", x));
            var item = new GenericIdentity(context.ClientId, OAuthDefaults.AuthenticationType);
            var identity = new ClaimsIdentity(item, scopes);
            context.Validated(identity);
            await base.GrantClientCredentials(context);
        }

        /// <summary>
        /// 生成 access_token（resource owner password credentials 授权方式）
        /// </summary>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            if (string.IsNullOrEmpty(context.UserName))
            {
                context.SetError("invalid_username", "username is not valid");
                return;
            }
            if (string.IsNullOrEmpty(context.Password))
            {
                context.SetError("invalid_password", "password is not valid");
                return;
            }

            if (context.UserName != "shoy" || context.Password != "123456")
            {
                context.SetError("invalid_identity", "username or password is not valid");
                return;
            }

            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            context.Validated(oAuthIdentity);
            await base.GrantResourceOwnerCredentials(context);
        }

        /// <summary>
        /// 生成 authorization_code（authorization code 授权方式）、生成 access_token （implicit 授权模式）
        /// </summary>
        public override async Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            if (context.AuthorizeRequest.IsImplicitGrantType)
            {
                //implicit 授权方式
                var identity = new ClaimsIdentity("Bearer");
                context.OwinContext.Authentication.SignIn(identity);
                context.RequestCompleted();
            }
            else if (context.AuthorizeRequest.IsAuthorizationCodeGrantType)
            {
                //authorization code 授权方式
                var state = context.Request.Query["state"] ?? string.Empty;
                var redirectUri = context.Request.Query["redirect_uri"] ?? "/code";
                var clientId = context.Request.Query["client_id"];
                var identity = new ClaimsIdentity(new GenericIdentity(
                    clientId, OAuthDefaults.AuthenticationType));

                var authorizeCodeContext = new AuthenticationTokenCreateContext(
                    context.OwinContext,
                    context.Options.AuthorizationCodeFormat,
                    new AuthenticationTicket(
                        identity,
                        new AuthenticationProperties(new Dictionary<string, string>
                        {
                            {"client_id", clientId},
                            {"redirect_uri", redirectUri}
                        })
                        {
                            IssuedUtc = DateTimeOffset.UtcNow,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(context.Options.AuthorizationCodeExpireTimeSpan)
                        }));

                await context.Options.AuthorizationCodeProvider.CreateAsync(authorizeCodeContext);
                var url = redirectUri + "?code=" + authorizeCodeContext.Token;
                if (!string.IsNullOrWhiteSpace(state))
                    url += $"&state={state}";
                context.Response.Redirect(url);
                context.RequestCompleted();
            }
        }

        /// <summary>
        /// 验证 authorization_code 的请求
        /// </summary>
        public override async Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            if (context.AuthorizeRequest.ClientId == "shoy" &&
                (context.AuthorizeRequest.IsAuthorizationCodeGrantType || context.AuthorizeRequest.IsImplicitGrantType))
            {
                context.Validated();
            }
            else
            {
                context.Rejected();
            }
        }

        /// <summary>
        /// 验证 redirect_uri
        /// </summary>
        public override async Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            context.Validated(context.RedirectUri ?? "/code");
            await base.ValidateClientRedirectUri(context);
        }

        ///// <summary>
        ///// 验证 access_token 的请求
        ///// </summary>
        //public override async Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        //{
        //    if (context.TokenRequest.IsAuthorizationCodeGrantType || context.TokenRequest.IsResourceOwnerPasswordCredentialsGrantType || context.TokenRequest.IsRefreshTokenGrantType)
        //    {
        //        context.Validated();
        //    }
        //    else
        //    {
        //        context.Rejected();
        //    }
        //}
    }
}