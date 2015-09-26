
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Shoy.Core.Reflection
{
    public abstract class DefaultAssemblyFinder : IAssemblyFinder
    {
        private readonly Func<Assembly, bool> _defaultPredicate;

        protected DefaultAssemblyFinder(Func<Assembly, bool> defaultPredicate = null)
        {
            _defaultPredicate = defaultPredicate;
        }

        public IEnumerable<Assembly> FindAll()
        {
            var path = AppDomain.CurrentDomain.RelativeSearchPath;
            if (!Directory.Exists(path))
                path = AppDomain.CurrentDomain.BaseDirectory;
            var asses = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom).ToArray();
            return _defaultPredicate != null ? asses.Where(_defaultPredicate) : asses;
        }

        public IEnumerable<Assembly> Find(Func<Assembly, bool> expression)
        {
            return FindAll().Where(expression);
        }
    }
}
