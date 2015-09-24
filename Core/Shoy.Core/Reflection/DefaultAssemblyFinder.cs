
using System;
using System.Collections.Generic;
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
            var asses = AppDomain.CurrentDomain.GetAssemblies();
            return _defaultPredicate != null ? asses.Where(_defaultPredicate) : asses;
        }

        public IEnumerable<Assembly> Find(Func<Assembly, bool> expression)
        {
            return FindAll().Where(expression);
        }
    }
}
