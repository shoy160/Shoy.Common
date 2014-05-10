using System;
using System.Web;
using System.Configuration;

namespace Shoy.UrlRewriter
{
	internal class RewriterUtils
	{
		internal static void RewriteUrl(HttpContext context, string sendToUrl)
		{
			string x, y;
			RewriteUrl(context, sendToUrl, out x, out y);
		}

		internal static void RewriteUrl(HttpContext context, string sendToUrl, out string sendToUrlLessQString, out string filePath)
		{
			if (context.Request.QueryString.Count > 0)
			{
				if (sendToUrl.IndexOf('?') != -1)
					sendToUrl += "&" + context.Request.QueryString;
				else
					sendToUrl += "?" + context.Request.QueryString;
			}
			string queryString = String.Empty;
			sendToUrlLessQString = sendToUrl;
			if (sendToUrl.IndexOf('?') >= 0)
			{
				sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf('?'));
				queryString = sendToUrl.Substring(sendToUrl.IndexOf('?') + 1);
			}
			filePath = context.Server.MapPath(sendToUrlLessQString);
			context.RewritePath(sendToUrlLessQString, String.Empty, queryString);
		}

		internal static string ResolveUrl(string appPath, string url)
		{
            if (url.Length == 0 || url[0] != '~')
            {
                return url + "(\\?[^\\?]+)?";		// there is no ~ in the first character position, just return the url
            }
		    if (url.Length == 1)
		        return appPath;  // there is just the ~ in the URL, return the appPath
		    if (url[1] == '/' || url[1] == '\\')
		    {
		        // url looks like ~/ or ~\
		        if (appPath.Length > 1)
		            return appPath + "/" + url.Substring(2);
		        return "/" + url.Substring(2);
		    }
		    // url looks like ~something
		    if (appPath.Length > 1)
		        return appPath + "/" + url.Substring(1);
		    return appPath + url.Substring(1);
		}

        internal static RewriterSection GetSection()
        {
            if (HttpContext.Current.Cache["RewriterSection"] == null)
                HttpContext.Current.Cache.Insert("RewriterSection", ConfigurationManager.GetSection("RewriterSection"));
            return (RewriterSection) HttpContext.Current.Cache["RewriterSection"];
        }
	}
}
