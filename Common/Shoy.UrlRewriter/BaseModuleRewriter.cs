using System;
using System.Web;

namespace Shoy.UrlRewriter
{
	public abstract class BaseModuleRewriter : IHttpModule
	{
		public virtual void Init(HttpApplication app)
		{
			app.AuthorizeRequest += BaseModuleRewriterAuthorizeRequest;
		}

		public virtual void Dispose() {}

		protected virtual void BaseModuleRewriterAuthorizeRequest(object sender, EventArgs e)
		{
			var app = (HttpApplication) sender;
            //·º½âÎö
			Rewrite(app.Request.Url.AbsoluteUri, app);
		}

	    protected abstract void Rewrite(string requestedPath, HttpApplication app);
	}
}
