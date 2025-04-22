using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using WebStore.Entities;

namespace WebStore
{
    public class WebStoreContextFactory : IDesignTimeDbContextFactory<WebStoreContext>
    {
        public WebStoreContext CreateDbContext(string[] args)
        {
            // Build the configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Set up DbContextOptions with the configuration
            var optionsBuilder = new DbContextOptionsBuilder<WebStoreContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("WebStoreDb"));

            // Return the WebStoreContext with options and configuration
            return new WebStoreContext(optionsBuilder.Options, configuration);
        }
    }
}
