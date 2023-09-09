using UrlShortner.Clients.Implementations;
using UrlShortner.Clients.Interfaces;

namespace UrlShortner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

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