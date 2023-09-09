using UrlShortner.Clients.Implementations;
using UrlShortner.Clients.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace UrlShortner
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
            ApplicationSettings.UserId = builder.Configuration.GetValue<string>("ApplicationSettings:UserId");
            ApplicationSettings.SecureConnectionBundlePath = builder.Configuration.GetValue<string>("ApplicationSettings:SecureConnectionBundlePath");
            ApplicationSettings.SecurePassword = builder.Configuration.GetValue<string>("ApplicationSettings:SecurePassword");
            
            ApplicationSettings.ConnectionString = builder.Configuration.GetValue<string>("ApplicationSettings:StorageAccountConnection"); // Replace with your Azure Blob Storage connection string
            ApplicationSettings.ContainerName = builder.Configuration.GetValue<string>("ApplicationSettings:ContainerName");       // Replace with your container name
            ApplicationSettings.BlobName = builder.Configuration.GetValue<string>("ApplicationSettings:BlobName");                // Replace with the name of the blob you want to download
            string localFilePath = $"/home/site/wwwroot/{ApplicationSettings.BlobName}";           // Replace with the local file path where you want to save the downloaded file


            if (!File.Exists(localFilePath))
            {
                BlobServiceClient blobServiceClient = new(ApplicationSettings.ConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ApplicationSettings.ContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(ApplicationSettings.BlobName);

                BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

                using (FileStream fs = File.OpenWrite(localFilePath))
                {
                    await blobDownloadInfo.Content.CopyToAsync(fs);
                    fs.Close();
                }
            }

            
            builder.Services.AddControllersWithViews();
            // Add MVC services to the container.
            builder.Services.AddMvc();
            builder.Services.AddSingleton<ICassandraCluster, CassandraCluster>();
            builder.Services.AddScoped<IUrlClient, UrlClient>();
            
            var app = builder.Build();

            app.UseRouting();

            //app.MapControllerRoute(name: "default", 
            //    pattern:"{controller=Home}/{action=Index}/{code?}");

            app.MapControllerRoute(name: "default",
                pattern: "/",
                defaults: new { controller = "Home", action = "Index" });

            app.MapControllerRoute(name: "redirectUrl",
                pattern: "/{code?}",
                defaults: new { controller = "Home", action = "RedirectUrl" });

            
            //app.UseEndpoints(endpoints =>
            //{
            //    //Configuring the MVC middleware to the request processing pipeline
            //    endpoints.MapDefaultControllerRoute();
            //});
            app.Run();
        }
    }
}