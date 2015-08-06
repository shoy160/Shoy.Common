using MongoDB.Bson;

namespace Shoy.FileSystem.Domain
{
    public class FileDocument
    {
        public string Id { get; set; }
        public BsonValue File { get; set; }
    }
}