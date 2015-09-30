using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace DayEasy.EntityFramework
{
    /// <summary> 数据库初始化操作类 </summary>
    public static class DatabaseInitializer
    {
        /// <summary> EF预热，解决EF6第一次加载慢的问题 </summary>
        public static void Initialize(DbContext context)
        {
            using (context)
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappingItemCollection =
                    (StorageMappingItemCollection)objectContext.ObjectStateManager
                        .MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
                mappingItemCollection.GenerateViews(new List<EdmSchemaError>());
            }
        }
    }
}
