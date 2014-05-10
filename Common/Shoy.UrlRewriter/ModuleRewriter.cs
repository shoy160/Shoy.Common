using System.Text.RegularExpressions;
using System.Web;

namespace Shoy.UrlRewriter
{
	public class ModuleRewriter : BaseModuleRewriter
	{
		protected override void Rewrite(string requestedPath, HttpApplication app)
		{
			//app.Context.Trace.Write("ModuleRewriter", "Entering ModuleRewriter");
		    var rules = RewriterUtils.GetSection().Rules;
		    var host = "http://" + app.Context.Request.ServerVariables["HTTP_HOST"];
			for(int i = 0; i < rules.Count; i++)
			{
                //·º½âÎö
			    string lookFor = "^" + RewriterUtils.ResolveUrl(host, rules[i].LookFor) + "$";

				// Create a regex
				var re = new Regex(lookFor, RegexOptions.IgnoreCase);

				// See if a match is found
				if (re.IsMatch(requestedPath))
				{
					// match found - do any replacement needed
					string sendToUrl = RewriterUtils.ResolveUrl(app.Context.Request.ApplicationPath, re.Replace(requestedPath, rules[i].SendTo));

					// log rewriting information to the Trace object
					app.Context.Trace.Write("ModuleRewriter", "Rewriting URL to " + sendToUrl);

					// Rewrite the URL
					RewriterUtils.RewriteUrl(app.Context, sendToUrl);
					break;		// exit the for loop
				}
			}

			// Log information to the Trace object
			app.Context.Trace.Write("ModuleRewriter", "Exiting ModuleRewriter");
		}
	}
}
