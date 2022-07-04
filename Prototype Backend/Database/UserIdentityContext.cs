using Microsoft.EntityFrameworkCore;
using Prototype_Backend.Models;
using Azure.Identity;

namespace Prototype_Backend.Database
{
    public class UserIdentityContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        protected readonly IConfigurationBuilder ConfigurationBuilder;

        public UserIdentityContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server with connection string from app settings
            //if (Program.InProduction) options.UseSqlServer($"https://{Configuration["KeyVaultName"]}.vault.azure.net/");
            //else 
                options.UseSqlServer(Configuration["ConnectionString"]);
        }
        internal DbSet<User> Users { get; set; }
        internal DbSet<Identity> Identities { get; set; }
        
    }
}
