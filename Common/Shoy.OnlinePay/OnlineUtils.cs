using System.Reflection;
using System.Web;
namespace Shoy.OnlinePay
{
    public abstract class OnlineUtils
    {
        public static OnlineUtils GetInstance(PayType type)
        {
            var name = type.ToString();
            OnlineUtils instance;
            if (!string.IsNullOrEmpty(name))
            {
                var ass = Assembly.Load("Shoy.OnlinePay");
                instance =
                    (OnlineUtils)
                    ass.CreateInstance("Shoy.OnlinePay." + name + ".Base");
            }
            else
                instance = null;
            return instance;
        }

        public abstract string CreateUrl(ParameterInfo info);

        public abstract BaseResult VerifyCallBack(HttpRequest request);

    }
}
