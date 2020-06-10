using AdaptiveProvider.Core;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaptiveProvider.Data
{
    public class ResourceRepository : IDisposable, IResourceRepository
    {
        private bool disposedValue;
        private LiteDatabase _db;

        public ResourceRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"ResourceRepository: {nameof(connectionString)} cannot be empty or whitespace");
            }

            _db = new LiteDatabase(connectionString);
            BsonMapper.Global.Entity<CloudResource>().Id(r => r.Id);
        }

        public CloudResource Get(string type, string id)
        {
            var col = _db.GetCollection<CloudResource>(type);

            return col.FindOne(x => x.Id == id);
        }

        public string Add(string type, CloudResource resource)
        {
            var col = _db.GetCollection<CloudResource>(type);
            var x = col.Insert(resource);

            return x.AsString;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _db.Dispose();
                }

                _db = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
