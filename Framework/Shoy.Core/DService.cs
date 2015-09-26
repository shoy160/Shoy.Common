using Shoy.Core.Domain;

namespace Shoy.Core
{
    /// <summary> 服务基类 </summary>
    public abstract class DService
    {
        protected DService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; private set; }
    }
}
