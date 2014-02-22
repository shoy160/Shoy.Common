using System.Configuration;

namespace Shoy.UrlRewriter
{
    public class RewriterSection:ConfigurationSection
    {
        [ConfigurationProperty("Rules", IsDefaultCollection = false)]
        internal Rules Rules
        {
            get { return (Rules)base["Rules"]; }
        }
    }

    internal class Rule:ConfigurationElement
    {
        [ConfigurationProperty("LookFor", DefaultValue = "", IsRequired = false)]
        public string LookFor
        {
            get { return (string)base["LookFor"]; }
            set { base["LookFor"] = value; }
        }

        [ConfigurationProperty("SendTo", DefaultValue = "", IsRequired = false)]
        public string SendTo
        {
            get { return (string)base["SendTo"]; }
            set { base["SendTo"] = value; }
        }
    }

    internal class Rules : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Rule();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var rule = element as Rule;
            if (rule == null)
                return "";
            return rule.LookFor;
        }

        public Rule this[int index]
        {
            get { return (Rule)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }
    }

}
