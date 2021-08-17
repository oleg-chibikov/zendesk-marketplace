using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;

namespace OlegChibikov.ZendeskInterview.Marketplace.DAL
{
    public class LiteDbRepository<TEntity> : ILiteDbRepository<TEntity>
    {
        readonly bool _initialDataLoadRequired;
        readonly IResourceLoader<TEntity> _resourceLoader;
        readonly ILiteDatabase _liteDatabase;
        readonly ILiteCollection<TEntity> _liteCollection;
        readonly IRelatedEntitiesLoader? _relatedEntitiesLoader;

        public LiteDbRepository(IResourceLoader<TEntity> resourceLoader, IRelatedEntitiesLoader? relatedEntitiesLoader)
        {
            _resourceLoader = resourceLoader ?? throw new ArgumentNullException(nameof(resourceLoader));
            _relatedEntitiesLoader = relatedEntitiesLoader;
            var type = typeof(TEntity);
            var path = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty), $"{type.Name}.db");
            _initialDataLoadRequired = !File.Exists(path);
            _liteDatabase = new LiteDatabase(path);
            _liteCollection = _liteDatabase.GetCollection<TEntity>(type.Name);
        }

        public IEnumerable<object> Find(string propertyName, object? value, bool isCollection)
        {
            _ = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

            var queryPropertyName = (propertyName == "Id" ? "_id" : propertyName) + (isCollection ? "[*] ANY" : null);
            var bson = value is DateTimeOffset ? BsonMapper.Global.Serialize(value) : new BsonValue(value);
            var entities = _liteCollection.Find(Query.EQ(queryPropertyName, bson)).Cast<object>().ToArray();
            if (_relatedEntitiesLoader != null)
            {
                foreach (var entity in entities)
                {
                    _relatedEntitiesLoader.LoadRelatedEntities(entity);
                }
            }

            return entities;
        }

        public TEntity? FindById<TId>(TId value)
        {
            var entity = _liteCollection.FindById(new BsonValue(value));
            if (entity == null)
            {
                return default;
            }

            if (_relatedEntitiesLoader != null)
            {
                _relatedEntitiesLoader.LoadRelatedEntities(entity);
            }

            return entity;
        }

        public async Task LoadInitialDataIfNeededAsync()
        {
            if (!_initialDataLoadRequired)
            {
                return;
            }

            // Using reflection to build indexes for every field including collection fields
            foreach (var propertyInfo in typeof(TEntity).GetProperties())
            {
                var collectionIndexMarker = propertyInfo.IsCollection() ? "[*]" : null;
                _liteCollection.EnsureIndex(propertyInfo.Name + collectionIndexMarker);
            }

            var entities = await _resourceLoader.LoadEntitiesAsync().ConfigureAwait(false);
            _liteCollection.InsertBulk(entities);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _liteDatabase.Dispose();
            }
        }
    }
}