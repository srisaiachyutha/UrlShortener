namespace UrlShortner.Models
{
    public class EncodeUrlResponse
    {
        public string OriginalUrl { get; set; } = null!;
        public string EncodedCode { get; set; } = null!;
    }
}
