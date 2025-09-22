using CryptoDashboard.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace CryptoDashboard.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<DateRangeLog> _collection;

        public MongoDbService(IConfiguration configuration)
        {
            var settings = configuration.GetSection("MongoSettings");

           
            var connectionString = settings.GetValue<string>("ConnectionString");
            var databaseName = settings.GetValue<string>("Database");
            var collectionName = settings.GetValue<string>("DateRangeCollection");

            
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Mongo connection string is null! Check appsettings.json");

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new Exception("Mongo database name is null! Check appsettings.json");

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new Exception("Mongo collection name is null! Check appsettings.json");

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<DateRangeLog>(collectionName);
        }

        public void LogDateRange(DateTime beginDate, DateTime endDate)
        {
            var log = new DateRangeLog
            {
                Id = Guid.NewGuid(),
                BeginDate = beginDate,
                EndDate = endDate,
                RequestDate = DateTime.UtcNow
            };

            _collection.InsertOne(log);
        }
    }
}