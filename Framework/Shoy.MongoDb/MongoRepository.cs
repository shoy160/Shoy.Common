using System;
using System.Linq;
using System.Linq.Expressions;
using DayEasy.Core;
using DayEasy.Core.Domain.Entities;
using DayEasy.Core.Domain.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace DayEasy.MongoDb
{
    public class MongoRepository<TEntity> : DRepository<TEntity, int>, IDependency
        where TEntity : DEntity<int>
    {
        private readonly IMongoProvider _databaseProvider;

        protected MongoDatabase Database
        {
            get { return _databaseProvider.Database; }
        }

        protected MongoCollection<TEntity> Collection
        {
            get { return _databaseProvider.Database.GetCollection<TEntity>(typeof(TEntity).Name); }
        }

        public MongoRepository(IMongoProvider unitOfWork)
            : base(unitOfWork)
        {
            _databaseProvider = unitOfWork;
        }

        public override IQueryable<TEntity> Table
        {
            get { return Collection.AsQueryable(); }
        }

        public override int Insert(TEntity entity)
        {
            var result = Collection.Insert(entity);
            return (int)result.DocumentsAffected;
        }

        public override int Delete(TEntity entity)
        {
            return Delete(entity.Id);
        }

        public override int Delete(int key)
        {
            return (int)Collection.Remove(Query.EQ("id", key)).DocumentsAffected;
        }

        public override int Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Update(TEntity entity, Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
