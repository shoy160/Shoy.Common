using System.Configuration;

namespace Shoy.Services
{
    public class MissionSection : ConfigurationSection
    {
        [ConfigurationProperty("Missions", IsDefaultCollection = false)]
        public Missions Missions
        {
            get { return (Missions) base["Missions"]; }
        }
    }

    /// <summary>
    /// 单个任务配置
    /// </summary>
    public class Mission : ConfigurationElement
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true)]
        public string Name
        {
            get { return (string) base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// 任务类型(必须实现IMission)
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        public string Type
        {
            get { return (string) base["type"]; }
            set { base["type"] = value; }
        }

        /// <summary>
        /// 执行周期(分钟)
        /// </summary>
        [ConfigurationProperty("interval", DefaultValue = 10, IsRequired = true)]
        public int Interval
        {
            get { return (int) base["interval"]; }
            set { base["interval"] = value; }
        }
    }

    /// <summary>
    /// 任务列表
    /// </summary>
    public class Missions : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Mission();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var mission = element as Mission;
            if (mission == null)
                return "";
            return mission.Name;
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Add(ConfigurationElement element)
        {
            BaseAdd(element);
        }

        public Mission this[int index]
        {
            get { return (Mission) BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }
    }
}
