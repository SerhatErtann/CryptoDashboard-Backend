using CryptoDashboard.Model;
using Npgsql;
using System.Diagnostics.Tracing;
using static CryptoDashboard.Model.CryptoPrice;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CryptoDashboard.Services
{
    public class CryptoService
    {
        private readonly string? _connectionString;

        public CryptoService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public enum Period
        {
            weekly, 
            monthly,
            daily 
        }

        public List<CryptoDataModel> GetCryptoDataFiltered(

            CryptoDataRequest cryptoDataRequest 
)
        {
            var list = new List<CryptoDataModel>();
           
            string groupBy = cryptoDataRequest.Period switch
            {

                CryptoService.Period.weekly =>"week",
                CryptoService.Period.monthly => "month",
                CryptoService.Period.daily => "day" ,
                _ => "day"
            };

            DateTime endDateParam = (cryptoDataRequest.endDate ?? DateTime.Now).Date;
            int days = cryptoDataRequest.range ?? 30;
            DateTime startDateParam = (cryptoDataRequest.startDate ?? endDateParam.AddDays(-days)).Date;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string query = $@"
            SELECT date_trunc('{groupBy}', ""Date"") AS ""Date"",
                   AVG(""Price"") AS ""Price"",
                   AVG(""Open"") AS ""Open"",
                   AVG(""High"") AS ""High"",
                   AVG(""Low"") AS ""Low"",
                   SUM(""Vol"") AS ""Vol"",
                   AVG(""Change_percent"") AS ""Change_percent"",
                   ""CoinName""
            FROM ""CryptoPrice""
            WHERE LOWER(""CoinName"") LIKE LOWER(@coin || '%')
              AND ""Date"" BETWEEN @start AND @end";

                if (cryptoDataRequest.minPrice.HasValue)
                    query += " AND \"Price\" >= @minPrice";
                if (cryptoDataRequest.maxPrice.HasValue)
                    query += " AND \"Price\" <= @maxPrice";

                query += $@"
            GROUP BY date_trunc('{groupBy}', ""Date""), ""CoinName""
            ORDER BY date_trunc('{groupBy}', ""Date"");";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("coin", cryptoDataRequest.CoinName);
                    cmd.Parameters.AddWithValue("start", startDateParam);
                    cmd.Parameters.AddWithValue("end", endDateParam);

                    if (cryptoDataRequest.maxPrice.HasValue) cmd.Parameters.AddWithValue("minPrice", cryptoDataRequest.minPrice.Value);
                    if (cryptoDataRequest.maxPrice.HasValue) cmd.Parameters.AddWithValue("maxPrice", cryptoDataRequest.maxPrice.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CryptoDataModel
                            {
                                Date = reader.GetDateTime(0),
                                Price = reader.GetDecimal(1),
                                Open = reader.GetDecimal(2),
                                High = reader.GetDecimal(3),
                                Low = reader.GetDecimal(4),
                                Vol = reader.GetDecimal(5),
                                Change_percent = reader.GetDecimal(6),
                                CoinName = reader.GetString(7)
                            });
                        }
                    }
                }
            }

            return list;
        }
     
        public List<CryptoDataModel> GetCryptoData(string coinName, DateTime startDate, DateTime endDate)
        {
            var list = new List<CryptoDataModel>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                var query = @"
              SELECT ""Date"", ""Price"", ""Open"", ""High"", ""Low"", ""Vol"", ""Change_percent"", ""CoinName""
              FROM ""CryptoPrice""
              WHERE LOWER(""CoinName"") LIKE LOWER(@coin || '%')
                AND ""Date"" BETWEEN @start AND @end
              ORDER BY ""Date"" DESC;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("coin", coinName);
                    cmd.Parameters.AddWithValue("start", startDate);
                    cmd.Parameters.AddWithValue("end", endDate);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CryptoDataModel
                            {
                                Date = reader.GetDateTime(0),
                                Price = reader.GetDecimal(1),
                                Open = reader.GetDecimal(2),
                                High = reader.GetDecimal(3),   
                                Low = reader.GetDecimal(4),
                                Vol = reader.GetDecimal(5),
                                Change_percent = reader.GetDecimal(6),
                                CoinName = reader.GetString(7)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public object GetCryptoStats(DateTime startDate, DateTime endDate)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                var sql = @"
                 SELECT 
                     ""CoinName"",
                     MIN(""Price"") AS MinPrice,
                     MAX(""Price"") AS MaxPrice,
                     AVG( ((""Price"" - ""Open"") / NULLIF(""Open"",0)) * 100 ) AS AvgDiffPercent
                 FROM ""CryptoPrice""
                 WHERE ""Date"" BETWEEN @start AND @end
                 GROUP BY ""CoinName""
                 ORDER BY AvgDiffPercent DESC
                 LIMIT 1;";


                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("start", startDate);
                    cmd.Parameters.AddWithValue("end", endDate);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new
                            {
                                MinPrice = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1),
                                MaxPrice = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                                AvgDiffPercent = reader.IsDBNull(3) ? 0 : reader.GetDouble(3)
                            };
                        }
                    }
                }
            }
            return null!;
        }
    }
}
