using System;
using Shoy.AutoMapper.Attributes;
using Shoy.Core.Modules;
using Shoy.Core.Reflection;
using Shoy.Utility.Logging;

namespace Shoy.AutoMapper
{
    public class AutoMapperModule : DModule
    {
        private readonly ILogger _logger = LogManager.Logger<AutoMapperModule>();
        private static bool _isCreating;
        private static readonly object LockObj = new object();

        public override void PreInitialize()
        {
            if (_isCreating)
                return;
            lock (LockObj)
            {
                if (_isCreating)
                    return;
                _logger.Debug("AutoMapper PreInitialize...");
                RegisteAutoMapByAttribute();
                _isCreating = true;
            }
        }

        private void RegisteAutoMapByAttribute()
        {
            var types = IocManager.Resolve<ITypeFinder>().Find(
                type => type.IsDefined(typeof(AutoMapAttribute), false) ||
                        type.IsDefined(typeof(AutoMapFromAttribute), false) ||
                        type.IsDefined(typeof(AutoMapToAttribute), false));
            foreach (Type type in types)
            {
                AutoMapperHelper.CreateMap(type);
            }
        }
    }
}
