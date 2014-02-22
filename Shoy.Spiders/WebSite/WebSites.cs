
namespace Shoy.Spiders.WebSite
{
    public enum WebSites
    {
        Amazon = 1,
        Suning = 2,
        Buy360 = 3,
        Coo8 = 4,
        Gome = 5,
        NewEgg = 6
    }

    public static class WebSitesExtension
    {
        public static string GetValue(this WebSites site)
        {
            return site.ToString();
        }
    }
}
