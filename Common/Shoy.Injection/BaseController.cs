using System.Web.Mvc;

namespace Shoy.Injection
{
    public abstract class BaseController : Controller
    {
        protected T Resolve<T>()
        {
            return DiHelper.Resolve<T>();
        }
    }

    public abstract class BaseController<T> : BaseController
    {
        protected readonly T _service = DiHelper.Resolve<T>();
    }

    public abstract class BaseController<T, TV> : BaseController<T>
    {
        protected readonly TV _service02 = DiHelper.Resolve<TV>();
    }

    public abstract class BaseController<T, TV, TVV> : BaseController<T, TV>
    {
        protected readonly TVV _service03 = DiHelper.Resolve<TVV>();
    }
}
