using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using AutoMapper;

namespace Shoy.MvcDemo.AutoMapper
{
    public static class AutoMapExtensions
    {
        public static T MapTo<T>(this object entity)
        {
            if (entity == null) return default(T);
            Mapper.CreateMap(entity.GetType(), typeof(T));
            return Mapper.Map<T>(entity);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source,
            TDestination def = default(TDestination))
        {
            if (source == null) return def;
            Mapper.CreateMap<TSource, TDestination>();
            return Mapper.Map<TDestination>(source);
        }

        public static List<TDestination> MapToList<TDestination>(this IEnumerable source)
        {
            foreach (var first in source)
            {
                var type = first.GetType();
                Mapper.CreateMap(type, typeof(TDestination));
                break;
            }
            return Mapper.Map<List<TDestination>>(source);
        }

        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> sources,
            Action<IMappingOperationOptions> opts = null)
        {
            Mapper.CreateMap<TSource, TDestination>();
            if (opts == null)
                return Mapper.Map<List<TDestination>>(sources);
            return Mapper.Map<List<TDestination>>(sources, opts);
        }

        public static IEnumerable<T> DataReaderMapTo<T>(this IDataReader reader)
        {
            Mapper.Reset();
            Mapper.CreateMap<IDataReader, IEnumerable<T>>();
            return Mapper.Map<IDataReader, IEnumerable<T>>(reader);
        }
    }
}