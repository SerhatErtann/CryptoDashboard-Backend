using CryptoDashboard.Constant;
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
            IConfigurationSection settings = configuration.GetSection("MongoSettings");

            string? connectionString = settings.GetValue<string>("ConnectionString");
            string? databaseName = settings.GetValue<string>("Database");
            string? collectionName = settings.GetValue<string>("DateRangeCollection");

            if (string.IsNullOrWhiteSpace(connectionString))

                throw new Exception( GlobalConstants.Config);

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new Exception(GlobalConstants.MongoDatabase );

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new Exception(GlobalConstants.MongoCollection);

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