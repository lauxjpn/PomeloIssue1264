using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace WebApplicationTemplate
{
    public class IceCream
    {
        public int IceCreamId { get; set; }
        public string Name { get; set; }
    }

    public class Cookie
    {
        public int CookieId { get; set; }
        public string Name { get; set; }
    }

    public class CustomMySqlSqlGenerationHelper : MySqlSqlGenerationHelper
    {
        public CustomMySqlSqlGenerationHelper(
            RelationalSqlGenerationHelperDependencies dependencies,
            IMySqlOptions options)
            : base(dependencies, options)
        {
        }

        protected override string GetSchemaName(string name, string schema)
            => schema; // <-- this is the first part that is needed to map schemas to databases 
    }
    
    public class Context : DbContext
    {
        public virtual DbSet<IceCream> IceCreams { get; set; }
        public virtual DbSet<Cookie> Cookies { get; set; }

        public Context(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use this database for all tables that don't have another schema explicitly assigned.
            modelBuilder.HasDefaultSchema("Issue1264_IceCreamParlor");
            
            // Map the `Cookie` entity to the `Cookies` table on the `Issue1264_Bakery` database.
            modelBuilder.Entity<Cookie>().ToTable("Cookies", "Issue1264_Bakery");
            
            // Seed some test data.
            modelBuilder.Entity<IceCream>()
                .HasData(
                    new IceCream {IceCreamId = 1, Name = "Vanilla"},
                    new IceCream {IceCreamId = 2, Name = "Chocolate"});
            modelBuilder.Entity<Cookie>()
                .HasData(
                    new Cookie {CookieId = 1, Name = "Basic"},
                    new Cookie {CookieId = 2, Name = "Chocolate Chip"});
        }
    }
}