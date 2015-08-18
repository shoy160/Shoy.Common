using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Shoy.Data.EntityFramework;
using Shoy.Data.EntityFramework.Migrations;

namespace Shoy.Core.Data
{
    /// <summary>
    /// 数据库初始化操作类
    /// </summary>
    public class DatabaseInitializer
    {
        private static readonly ICollection<Assembly> MapperAssemblies = new List<Assembly>();

        /// <summary>
        /// 获取 数据实体映射配置信息集合
        /// </summary>
        public static ICollection<IEntityMapper> EntityMappers { get { return GetAllEntityMapper(); } }

        /// <summary>
        /// 设置数据库初始化，策略为自动迁移到最新版本
        /// </summary>
        public static void Initialize()
        {
            var context = new CodeFirstDbContext();
            IDatabaseInitializer<CodeFirstDbContext> initializer;
            if (!context.Database.Exists())
            {
                initializer = new CreateDatabaseIfNotExistsWithSeed();
            }
            else
            {
                initializer = new MigrateDatabaseToLatestVersion<CodeFirstDbContext, MigrationsConfiguration>();
            }
            Database.SetInitializer(initializer);
            
            //EF预热
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            var mappingItemCollection = (StorageMappingItemCollection)objectContext.ObjectStateManager
                .MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
            mappingItemCollection.GenerateViews(new List<EdmSchemaError>());
            context.Dispose();
        }

        /// <summary>
        /// 添加需要搜索实体映射的程序集到检索集合中
        /// </summary>
        public static void AddMapperAssembly(Assembly assembly)
        {
            if (MapperAssemblies.Any(m => m == assembly))
            {
                return;
            }
            MapperAssemblies.Add(assembly);
        }

        private static ICollection<IEntityMapper> GetAllEntityMapper()
        {
            Type baseType = typeof(IEntityMapper);
            Type[] mapperTypes = MapperAssemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) && type != baseType && !type.IsAbstract).ToArray();
            ICollection<IEntityMapper> result = mapperTypes.Select(type => Activator.CreateInstance(type) as IEntityMapper).ToList();
            return result;
        }
    }
}