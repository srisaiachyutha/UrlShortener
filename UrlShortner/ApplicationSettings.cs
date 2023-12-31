namespace UrlShortner
{
    public static class ApplicationSettings
    {
        public static string UserId { get; set; } = null!;
        public static string SecurePassword { get; set; } = null!;
        public static string SecureConnectionBundlePath { get; set; } = null!;
        public static string ConnectionString { get; set; } = null!;
        public static string ContainerName { get; set; } = null!;
        public static string BlobName { get; set; } = null!;
    }
}
