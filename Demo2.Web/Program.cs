using Demo2.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demo2.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();

            using (var scope = webHost.Services.CreateScope())
            {
                var dbc = scope.ServiceProvider.GetRequiredService<AppDatabaseContext>();
                dbc.Database.Migrate();
            }

            return webHost;
        }
    }
}
