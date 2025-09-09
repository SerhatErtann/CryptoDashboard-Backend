using CryptoDashboard.Model;
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
            var client = new MongoClient(settings["ConnectionString"]);
            var database = client.GetDatabase(settings["Database"]);
            _collection = database.GetCollection<DateRangeLog>(settings["DateRangeCollection"]);
        }

        public void LogDateRange(DateTime beginDate, DateTime endDate)
        {
            var log = new DateRangeLog
            {
                Id = Guid.NewGuid().ToString(),
                BeginDate = beginDate,
                EndDate = endDate,
                RequestDate = DateTime.UtcNow
            };

            _collection.InsertOne(log);
        }
    }
}
