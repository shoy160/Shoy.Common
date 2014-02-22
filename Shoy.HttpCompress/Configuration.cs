using System.Configuration;

namespace Shoy.HttpCompress
{
    public class Configuration : ConfigurationSection
    {
        [ConfigurationProperty("compressionType", IsRequired = false)]
        public CompressionType CompressionType
        {
            get
            {
                { return (base["compressionType"] == null) ? CompressionType.None : (CompressionType)base["compressionType"]; }
            }
            set
            {
                { base["compressionType"] = value; }
            }
        }

        [ConfigurationProperty("AutoCompress", IsDefaultCollection = false)]
        public AutoCompress AutoCompress
        {
            get { return (AutoCompress)base["AutoCompress"]; }
        }

        [ConfigurationProperty("ExcludedPaths", IsDefaultCollection = false)]
        public ExcludedPaths ExcludedPaths
        {
            get
            {
                return (ExcludedPaths)base["ExcludedPaths"];
            }
        }

        [ConfigurationProperty("ExcludedMimeTypes", IsDefaultCollection = false)]
        public ExcludedMimes ExcludedMimeTypes
        {
            get
            {
                return (ExcludedMimes)base["ExcludedMimeTypes"];
            }
        }

        [ConfigurationProperty("IncludedPaths", IsDefaultCollection = false)]
        public IncludedPaths IncludedPaths
        {
            get
            {
                return (IncludedPaths)base["IncludedPaths"];
            }
        }

        [ConfigurationProperty("IncludedMimeTypes", IsDefaultCollection = false)]
        public IncludedMimes IncludedMimeTypes
        {
            get
            {
                return (IncludedMimes)base["IncludedMimeTypes"];
            }
        }
    }

    public class AutoCompress:ConfigurationElement
    {
        public AutoCompress(){}
        public AutoCompress(bool js, bool css)
        {
            Js = js;
            Css = css;
        }

        [ConfigurationProperty("js", DefaultValue = false, IsRequired = false)]
        public bool Js
        {
            get { return (bool) base["js"]; }
            set { base["js"] = value; }
        }

        [ConfigurationProperty("css", DefaultValue = false, IsRequired = false)]
        public bool Css
        {
            get { return (bool)base["css"]; }
            set { base["css"] = value; }
        }
    }
       
    public class ExcludedPath : ConfigurationElement
    {

        public ExcludedPath() { }
        public ExcludedPath(string path)
        {
            Path = path;
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return (string)base["path"];
            }
            set
            {
                base["path"] = value;
            }
        }

    }

    public class ExcludedPaths : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExcludedPath();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ExcludedPath)element).Path;
        }

        public ExcludedPath this[int index]
        {
            get { return (ExcludedPath)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                    base.BaseRemoveAt(index);
                this.BaseAdd(index, value);
            }
        }

        public bool Contains(string path)
        {
            foreach (ExcludedPath ex in this)
            {
                if (ex.Path.ToLower().Equals(path.ToLower()))
                    return true;
            }
            return false;
        }
    }

    public class ExcludedMime : ConfigurationElement
    {

        public ExcludedMime() { }
        public ExcludedMime(string mime)
        {
            Mime = mime;
        }

        [ConfigurationProperty("mime", IsRequired = true)]
        public string Mime
        {
            get
            {
                return (string)base["mime"];
            }
            set
            {
                base["mime"] = value;
            }
        }

    }

    public class ExcludedMimes : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExcludedMime();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ExcludedMime)element).Mime;
        }

        public ExcludedMime this[int index]
        {
            get { return (ExcludedMime)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                    base.BaseRemoveAt(index);
                this.BaseAdd(index, value);
            }
        }

        public bool Contains(string mime)
        {
            foreach (ExcludedMime ex in this)
            {
                if (ex.Mime.ToLower().Equals(mime.ToLower()))
                    return true;
            }
            return false;
        }
    }

    public class IncludedMime : ConfigurationElement
    {

        public IncludedMime() { }
        public IncludedMime(string mime)
        {
            Mime = mime;
        }

        [ConfigurationProperty("mime", IsRequired = true)]
        public string Mime
        {
            get
            {
                return (string)base["mime"];
            }
            set
            {
                base["mime"] = value;
            }
        }

    }

    public class IncludedMimes : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new IncludedMime();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IncludedMime)element).Mime;
        }

        public IncludedMime this[int index]
        {
            get { return (IncludedMime)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                    base.BaseRemoveAt(index);
                this.BaseAdd(index, value);
            }
        }

        public bool Contains(string mime)
        {
            foreach (IncludedMime ex in this)
            {
                if (ex.Mime.ToLower().Equals(mime.ToLower()))
                    return true;
            }
            return false;
        }
    }

    public class IncludedPath : ConfigurationElement
    {

        public IncludedPath() { }
        public IncludedPath(string path)
        {
            Path = path;
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return (string)base["path"];
            }
            set
            {
                base["path"] = value;
            }
        }

    }

    public class IncludedPaths : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new IncludedPath();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IncludedPath)element).Path;
        }

        public IncludedPath this[int index]
        {
            get { return (IncludedPath)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                    base.BaseRemoveAt(index);
                this.BaseAdd(index, value);
            }
        }

        public bool Contains(string path)
        {
            foreach (IncludedPath ex in this)
            {
                if (ex.Path.ToLower().Equals(path.ToLower()))
                    return true;
            }
            return false;
        }
    }
    
}
