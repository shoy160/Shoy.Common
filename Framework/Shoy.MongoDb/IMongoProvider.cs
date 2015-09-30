
using DayEasy.Core.Domain;
using MongoDB.Driver;

namespace DayEasy.MongoDb
{
    public interface IMongoProvider : IUnitOfWork
    {
        MongoDatabase Database { get; }
    }
}
