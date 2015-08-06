using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using Shoy.FileSystem.Domain;
using Shoy.Utility.Helper;
using MongoCredential = MongoDB.Driver.MongoCredential;
using MongoServer = MongoDB.Driver.MongoServer;

namespace Shoy.FileSystem
{
    public class GridFsManager
    {
        private readonly string _database;

        private GridFsManager(string database)
        {
            _database = database;
        }

        internal static GridFsManager Instance(string database = "picture")
        {
            return new GridFsManager(string.Concat("file_", database));
        }

        /// <summary> Mongo服务器 </summary>
        /// <returns></returns>
        private MongoDatabase Server()
        {
            var server = new MongoServer(new MongoServerSettings
            {
                Servers = Contains.Config.Mongo.Servers.Select(t => new MongoServerAddress(t.Host, t.Port)),
                Credentials =
                    Contains.Config.Mongo.Credentials.Select(
                        t => MongoCredential.CreateCredential(t.DataBase, t.User, t.Pwd))
            });
            return server.GetDatabase(_database);
        }

        /// <summary> 保存图片 </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public string Save(Stream fileStream)
        {
            var db = Server();
            var name = IdHelper.Instance.Guid32;
            var col = db.GetCollection<FileDocument>("picture");
            var file = new FileDocument
            {
                Id = name,
                File = BsonValue.Create(fileStream)
            };
            
            col.Save(file);
            db.GridFS.Upload(fileStream, name);
            return name;
        }

        /// <summary> 读取文件 </summary>
        /// <param name="fileName"></param>
        public Stream Read(string fileName)
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

        public IOrderedEnumerable<MongoGridFSFileInfo> Find(IMongoQuery query = null)
        {
            var db = Server();
            if (query == null) query = Query.Empty;
            return db.GridFS.Find(query).OrderByDescending(t => t.UploadDate);
        }

        /// <summary> 删除文件 </summary>
        /// <param name="fileName"></param>
        public void Delete(string fileName)
        {
            var db = Server();
            db.GridFS.Delete(fileName);
        }
    }
}