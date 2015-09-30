using System.IO;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using Shoy.Core.Domain.Entities;
using Shoy.MongoDb.Configs;
using Shoy.Utility.Config;
using Shoy.Utility.Helper;

namespace Shoy.MongoDb
{
    /// <summary> MongoDB管理类 </summary>
    public class MongoManager
    {
        private readonly string _database;
        private string _collection;
        private const string Prefix = "dayeasy";

        public MongoManager(string database = Prefix, string collection = null)
        {
            _database = database;
            _collection = collection;
        }

        private MongoConfig Config
        {
            get
            {
                return ConfigUtils<MongoConfig>.Instance.Get();
            }
        }

        /// <summary> Mongo服务器 </summary>
        /// <returns></returns>
        private MongoDatabase Server()
        {
            var server = new MongoServer(new MongoServerSettings
            {
                Servers = Config.Servers.Select(t => new MongoServerAddress(t.Host, t.Port)),
                Credentials =
                    Config.Credentials.Select(
                        t => MongoCredential.CreateCredential(t.DataBase, t.User, t.Pwd))
            });
            return server.GetDatabase(_database);
        }

        /// <summary> 获取Mongo集合 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MongoCollection<T> Collection<T>() where T : DEntity
        {
            if (string.IsNullOrWhiteSpace(_collection))
            {
                _collection = string.Format("{0}.{1}", Prefix, typeof(T).Name.ToLower());
            }
            return Server().GetCollection<T>(_collection);
        }

        #region 文件管理

        /// <summary> 保存文件 </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public string SaveFile(Stream fileStream)
        {
            var db = Server();
            var name = IdHelper.Instance.Guid32;
            db.GridFS.Upload(fileStream, name);
            return name;
        }

        /// <summary> 读取文件 </summary>
        /// <param name="fileName"></param>
        public Stream ReadFile(string fileName)
        {
            try
            {
                var db = Server();
                var info = db.GridFS.FindOne(fileName);
                return info.Open(FileMode.Open);
            }
            catch
            {
                return Stream.Null;
            }
        }

        public IOrderedEnumerable<MongoGridFSFileInfo> FindFile(IMongoQuery query = null)
        {
            var db = Server();
            if (query == null) query = Query.Empty;
            return db.GridFS.Find(query).OrderByDescending(t => t.UploadDate);
        }

        /// <summary> 删除文件 </summary>
        /// <param name="fileName"></param>
        public void DeleteFile(string fileName)
        {
            var db = Server();
            db.GridFS.Delete(fileName);
        }

        #endregion
    }
}
