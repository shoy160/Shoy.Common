using System.ComponentModel;

namespace Shoy.AjaxHelper
{
    public enum ContentType
    {
        [Description("text/html")]
        Html = 0,
        [Description("text/xml")]
        Xml = 1,
        [Description("application/json")]
        Json = 2,
        [Description("text/html")]
        Text = 3
    }
}
