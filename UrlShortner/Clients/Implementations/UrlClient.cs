
using Cassandra;
using UrlShortner.Clients.Interfaces;

namespace UrlShortner.Clients.Implementations
{
    public class UrlClient : IUrlClient , IDisposable
    {
        private readonly ICassandraCluster _cassandraCluster;
        private readonly Cassandra.ISession? _session;
        private readonly string _keySpaceName = "tinyurl";
        
        public UrlClient(ICassandraCluster cassandraCluster)
        {
            _cassandraCluster = cassandraCluster;
            _session = CreateNewSession(_keySpaceName);
        }


        public string GetOriginalUrl(string code)
        {
            Cassandra.SimpleStatement simpleStatement = new("SELECT * FROM url WHERE code = ?", code);
            var rowSet = _session?.Execute(simpleStatement);
            string? originalUrl = null;
            foreach (var row in rowSet)
            {
                originalUrl = row.GetValue<string>("originalurl"); 
            }

            return originalUrl ?? string.Empty;
        }

        public string CreateSmallUrl(string originalUrl)
        {
            int maxTries = 3;
            for(int count = 0; count < maxTries; ++count)
            {
                var rowSet = _session?.Execute("SELECT * FROM id_generator WHERE item_id = 1");

                var nextId = rowSet!.First().GetValue<long>("next_id");

                // updating the next_id 
                SimpleStatement updateNextIdStatement = new("UPDATE id_generator SET next_id = ? WHERE item_id = 1 IF next_id = ?",
                nextId + 1, nextId);

                rowSet = _session?.Execute(updateNextIdStatement);


                var isNextIdUpdated = rowSet!.First().GetValue<bool>("[applied]");

                if (!isNextIdUpdated)
                    continue;
                
                nextId += 1;
                string code = ConvertLongToBase62(nextId);

                SimpleStatement insertUrlStatement = new("INSERT INTO url(code, originalurl) VALUES(?, ?) IF NOT EXISTS", code, originalUrl);

                rowSet = _session?.Execute(insertUrlStatement);
                bool isUrlInserted = rowSet!.First().GetValue<bool>("[applied]");
                if (isUrlInserted)
                    return code;
            }

            return string.Empty;
        }

        private Cassandra.ISession? CreateNewSession(string keySpace)
        {
            return _cassandraCluster.GetCluster().Connect(keySpace);
        }

        public void Dispose()
        {
            _session?.Dispose();
        }

        private static string ConvertLongToBase62(long id)
        {
            string ans = "";
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            while (id > 0)
            {
                ans += characters[(int)(id % 62)];
                id /= 62;
            }

            return ans;
        }

    }
}
