namespace Shoy.Services
{
    /// <summary> 任务基类 </summary>
    public abstract class MissionBase : IMission
    {
        protected bool IsAbort;

        public void Action()
        {
            Start();
        }

        public abstract void Start();

        public void Abort()
        {
            IsAbort = true;
        }

        public event ErrorHandler Error;

        public void Dispose()
        {
            Abort();
        }

        protected bool OnError(string msg)
        {
            if (Error == null) return false;
            var arg = new ErrorEventArg {Cancel = false, Message = msg};
            Error(this, arg);
            return arg.Cancel;
        }
    }
}
