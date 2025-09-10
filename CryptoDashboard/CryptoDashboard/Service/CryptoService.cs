using Npgsql;
using static CryptoDashboard.Model.CryptoPrice;

namespace CryptoDashboard.Services
{
    public class CryptoService
    {
        private readonly string? _connectionString;

        public CryptoService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }
        public List<CryptoDataModel> GetCryptoDataFiltered
 (
   string coinName,
   string? period = null,
   decimal? minPrice = null,
   decimal? maxPrice = null,
   string? sortColumn = null,
   string? sortOrder = null
 )
        {
            var list = new List<CryptoDataModel>();

            DateTime endDate = DateTime.Now.Date;

            int days = period switch
            {
                "7" => 7,
                "30" => 30,
                "90" => 90,
                _ => 30
            };

            DateTime startDate = endDate.AddDays(-days);

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
            SELECT ""Date"", ""Price"", ""Open"", ""High"", ""Low"", ""Vol"", ""Change_percent"", ""CoinName""
            FROM ""CryptoPrice""
            WHERE LOWER(""CoinName"") LIKE LOWER(@coin || '%')
              AND ""Date"" BETWEEN @start AND @end
        ";

                if (minPrice.HasValue)
                    query += " AND \"Price\" >= @minPrice";
                if (maxPrice.HasValue)
                    query += " AND \"Price\" <= @maxPrice";

                // --- Sıralama kontrolü ---
                var allowedColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "price", "\"Price\"" },
            { "date", "\"Date\"" },
            { "high", "\"High\"" },
            { "low", "\"Low\"" },
            { "vol", "\"Vol\"" }
        };

                string sortCol = allowedColumns.ContainsKey(sortColumn ?? "")
                    ? allowedColumns[sortColumn!]
                    : "\"Date\"";

                string sortDir = sortOrder?.ToLower() == "desc" ? "DESC" : "ASC";
                query += $" ORDER BY {sortCol} {sortDir};";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("coin", coinName);
                    cmd.Parameters.AddWithValue("start", startDate);
                    cmd.Parameters.AddWithValue("end", endDate);
                    if (minPrice.HasValue) cmd.Parameters.AddWithValue("minPrice", minPrice.Value);
                    if (maxPrice.HasValue) cmd.Parameters.AddWithValue("maxPrice", maxPrice.Value);

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
            if (list.Count == 0)
            {
                return new List<CryptoDataModel>
        {
            new CryptoDataModel
            {
                CoinName = coinName,
                Date = DateTime.Now,
                Price = 0,
                Open = 0,
                High = 0,
                Low = 0,
                Vol = 0,
                Change_percent = 0
            }
        };
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
