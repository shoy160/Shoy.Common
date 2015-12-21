using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Shoy.AutoMapper.Attributes;
using Shoy.Utility.Extend;

namespace Shoy.AutoMapper
{
    public static class AutoMapperHelper
    {
        public static void CreateMap(Type type)
        {
            CreateMap<AutoMapFromAttribute>(type);
            CreateMap<AutoMapToAttribute>(type);
            CreateMap<AutoMapAttribute>(type);
        }

        public static void CreateMap<TAttribute>(Type type)
            where TAttribute : AutoMapAttribute
        {
            if (!type.IsDefined(typeof(TAttribute)))
            {
                return;
            }

            foreach (var autoMapToAttribute in type.GetCustomAttributes<TAttribute>())
            {
                if (autoMapToAttribute.TargetTypes.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var targetType in autoMapToAttribute.TargetTypes)
                {
                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.To))
                    {
                        CreateMap(type, targetType, AutoMapDirection.To);
                    }

                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.From))
                    {
                        CreateMap(type, targetType, AutoMapDirection.From);
                    }
                }
            }
        }

        public static void CreateMap(Type sourceType, Type targetType, AutoMapDirection direction)
        {
            switch (direction)
            {
                case AutoMapDirection.To:
                    {
                        var map = Mapper.CreateMap(sourceType, targetType);
                        var props = sourceType.GetProperties().Where(t => t.IsDefined(typeof(MapFromAttribute)) && t != null);
                        foreach (var prop in props)
                        {
                            var mapfrom = prop.GetCustomAttribute<MapFromAttribute>();
                            map.ForMember(mapfrom.SourceName, opt => opt.MapFrom(prop.Name));
                        }
                    }
                    break;
                case AutoMapDirection.From:
                    {
                        var map = Mapper.CreateMap(targetType, sourceType);
                        var props = sourceType.GetProperties().Where(t => t.IsDefined(typeof(MapFromAttribute)));
                        foreach (var prop in props)
                        {
                            var mapfrom = prop.GetCustomAttribute<MapFromAttribute>();
                            map.ForMember(prop.Name, s => s.MapFrom(mapfrom.SourceName));
                        }
                    }
                    break;
            }
        }
        public static void CreateMapper<TTarget, TSource>()
        {
            Mapper.CreateMap<TTarget, TSource>();
        }
    }
}
