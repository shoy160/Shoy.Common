using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;

namespace Shoy.Open.OAuth
{
    public class OpenRefreshTokenProvider : AuthenticationTokenProvider
    {
        private static readonly ConcurrentDictionary<string, string> RefreshTokens = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 生成 refresh_token
        /// </summary>
        public override void Create(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(60);

            context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            RefreshTokens[context.Token] = context.SerializeTicket();
        }


        /// <summary>
        /// 由 refresh_token 解析成 access_token
        /// </summary>
        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (RefreshTokens.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }
    }
}