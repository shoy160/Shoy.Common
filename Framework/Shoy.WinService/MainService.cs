using System.ServiceProcess;

namespace Shoy.WinService
{
    public partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
            ServiceName = "DayEasy.UserService";
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
