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

        public LiteDbRepository(IResourceLoader<TEntity> resourceLoader)
        {
            _resourceLoader = resourceLoader ?? throw new ArgumentNullException(nameof(resourceLoader));
            var type = typeof(TEntity);
            var path = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty), $"{type.Name}.db");
            _initialDataLoadRequired = !File.Exists(path);
            _liteDatabase = new LiteDatabase(path);
            _liteCollection = _liteDatabase.GetCollection<TEntity>(type.Name);
        }

        public IEnumerable<object> Find(string propertyName, object value, bool isCollection)
        {
            var queryPropertyName = (propertyName == "Id" ? "_id" : propertyName) + (isCollection ? "[*] ANY" : null);
            var bson = value.GetType() == typeof(DateTimeOffset) ? BsonMapper.Global.Serialize(value) : new BsonValue(value);
            return _liteCollection.Find(Query.EQ(queryPropertyName, bson)).Cast<object>();
        }

        public TEntity? FindById<TId>(TId value)
        {
            return _liteCollection.FindById(new BsonValue(value));
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