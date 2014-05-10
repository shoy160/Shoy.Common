using System.Configuration;
using System.Linq;

namespace Shoy.MemCached
{
	/// <summary>
	/// Summary description for MemcachedConfigSection.
	/// </summary>
    public class MemcachedConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Servers", IsDefaultCollection = false)]
        public Servers Servers
        {
            get { return (Servers)base["Servers"]; }
        }
    }

    public class Server:ConfigurationElement
    {
        public Server(){}
        public Server(string name,string address)
        {
            Name = name;
            Address = address;
        }

        [ConfigurationProperty("Name", DefaultValue = "MyMemcache", IsRequired = false)]
        public string Name
        {
            get { return (string)base["Name"]; }
            set { base["Name"] = value; }
        }

        [ConfigurationProperty("Address", DefaultValue = "", IsRequired = false)]
        public string Address
        {
            get { return (string)base["Address"]; }
            set { base["Address"] = value; }
        }

        [ConfigurationProperty("Min", DefaultValue = 2, IsRequired = false)]
        public int MinConnections
        {
            get { return (int)base["Min"]; }
            set { base["Min"] = value; }
        }

        [ConfigurationProperty("Max", DefaultValue = 20, IsRequired = false)]
        public int MaxConnections
        {
            get { return (int)base["Max"]; }
            set { base["Max"] = value; }
        }

        [ConfigurationProperty("CTime", DefaultValue = 1000, IsRequired = false)]
        public int ConnectionTimeOut
        {
            get { return (int)base["CTime"]; }
            set { base["CTime"] = value; }
        }

        [ConfigurationProperty("STime", DefaultValue = 3000, IsRequired = false)]
        public int SocketTimeOut
        {
            get { return (int)base["STime"]; }
            set { base["STime"] = value; }
        }

        [ConfigurationProperty("Default", DefaultValue = false, IsRequired = false)]
        public bool Default
        {
            get { return (bool)base["Default"]; }
            set { base["Default"] = value; }
        }
    }

    public class Servers:ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Server();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var server = element as Server;
            if (server == null)
                return "";
            return server.Name;
        }
        public Server this[int index]
        {
            get { return (Server)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public bool Contains(string server)
        {
            return this.Cast<Server>().Any(ex => (ex.Name).ToLower().Equals(server.ToLower()));
        }
    }
}
