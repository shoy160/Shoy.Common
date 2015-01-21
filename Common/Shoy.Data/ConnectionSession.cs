using System.Configuration;

namespace Shoy.Data
{
    public class ConnectionSession : ConfigurationSection
    {
        [ConfigurationProperty("DbConnects", IsDefaultCollection = false)]
        internal DbConnects Connects
        {
            get { return (DbConnects)base["DbConnects"]; }
        }
    }

    internal class DbConnect : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsRequired = false)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("connectString", DefaultValue = "", IsRequired = false)]
        public string ConnectString
        {
            get { return (string)base["connectString"]; }
            set { base["connectString"] = value; }
        }

        [ConfigurationProperty("default", DefaultValue = false, IsRequired = false)]
        public bool IsDefault
        {
            get { return (bool)base["default"]; }
            set { base["default"] = value; }
        }

        [ConfigurationProperty("sqlType", DefaultValue = "MsSql", IsRequired = false)]
        public string ServerType
        {
            get { return (string)base["sqlType"]; }
            set { base["sqlType"] = value; }
        }
    }

    internal class DbConnects:ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new DbConnect();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var server = element as DbConnect;
            if (server == null)
                return string.Empty;
            return server.Name;
        }

        public DbConnect this[int index]
        {
            get { return (DbConnect)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }
    }
}
