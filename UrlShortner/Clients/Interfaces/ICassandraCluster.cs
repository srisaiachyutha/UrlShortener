namespace UrlShortner.Clients.Interfaces
{
    public interface ICassandraCluster
    {
        public Cassandra.ICluster GetCluster();
    }
}
