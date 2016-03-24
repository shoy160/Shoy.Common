using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Shoy.Utility.Config;

namespace Shoy.Wiki.Models.Config
{
    [Serializable]
    [XmlRoot("menus")]
    [FileName("menus.config")]
    public class MenuConfig : ConfigBase
    {
        public MenuConfig()
        {
            Groups = new MenuGroup[] { };
        }
        [XmlArray("groups")]
        [XmlArrayItem("group")]
        public MenuGroup[] Groups { get; set; }
    }

    [Serializable]
    public class MenuGroup : MenuItem
    {
        public MenuGroup()
        {
            Menus = new List<MenuItem>();
        }
        [XmlArray("menus")]
        [XmlArrayItem("menu")]
        public List<MenuItem> Menus { get; set; }
    }

    [Serializable]
    public class MenuItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        [XmlAttribute("info")]
        public string Info { get; set; }

        [XmlAttribute("permission")]
        public long Permission { get; set; }
    }
}
