using Cassandra;
using Microsoft.Extensions.Options;
using UrlShortner.Clients.Interfaces;

namespace UrlShortner.Clients.Implementations
{
    public class CassandraCluster : ICassandraCluster, IDisposable
    {
        private readonly ICluster _cluster;
        private readonly ApplicationSettings _applicationSettings;
        public CassandraCluster(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings.Value;
            _cluster = CreateCluster();
        }

        //private static ICluster CreateClusters()
        //{
        //    var cluster = Cluster.Builder()

        //    .WithCloudSecureConnectionBundle(@"C:\Users\psrisaiachyutha\Downloads\secure-connect-db-tinyurl.zip")
        //    .WithCredentials("gKMSkcnSNzcLwhLAWjxLQRpv",
        //        "6Af.WT2lzUdbO5ev5SU75AHeaDro__DD8oCZ1H1UqbdBPWKqjd6cgN5bAbusc-Z0D7c7wtE6bK2D-CJDdjmA6FCwB8_Q3M_fCy+BjU7nAZRZ6zuj0fpaSqk5qT59Poho")
        //    .Build();
        //    return cluster;
        //}

        private  ICluster CreateCluster()
        {
            var cluster = Cluster.Builder()
                //.WithCloudSecureConnectionBundle(@"C:\Users\psrisaiachyutha\Downloads\secure-connect-db-tinyurl.zip")

            .WithCloudSecureConnectionBundle(_applicationSettings.SecureConnectionBundlePath)
            .WithCredentials(
                _applicationSettings.UserId, 
                _applicationSettings.SecurePassword)
            .Build();
            return cluster;
        }

        public void Dispose()
        {
            _cluster.Dispose();
        }

        public ICluster GetCluster()
        {
            return _cluster;
        }
    }
}
