namespace UrlShortner.Clients.Interfaces
{
    public interface IUrlClient
    {
        public string GetOriginalUrl(string code);
        public string CreateSmallUrl(string originalUrl);
    }
}
