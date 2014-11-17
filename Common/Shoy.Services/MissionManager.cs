using System.Collections.Generic;
using System.Configuration;

namespace Shoy.Services
{
    public class MissionManager : IMission
    {
        private readonly List<MissionItem> _missions = new List<MissionItem>();
        private bool _complete = true;

        public void Action()
        {
            _complete = false;
            var list = ConfigurationManager.GetSection("DeyiMission") as MissionSection;
            if (list == null || list.Missions.Count == 0)
            {
                _complete = true;
                return;
            }
            Abort();
            foreach (Mission mission in list.Missions)
            {
                var item = new MissionItem(mission);
                _missions.Add(item);
                item.Start();
            }
            _complete = true;
        }

        public void Abort()
        {
            foreach (MissionItem mission in _missions)
            {
                mission.Stop();
            }
            _missions.Clear();
        }

        public event ErrorHandler Error;

        public void Dispose()
        {
            Abort();
        }

        public bool Complete()
        {
            return _complete;
        }
    }
}
