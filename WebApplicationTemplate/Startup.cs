using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace WebApplicationTemplate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            
            var efCoreServiceProvider = new ServiceCollection()
                .AddEntityFrameworkMySql()
                .AddSingleton<ISqlGenerationHelper, CustomMySqlSqlGenerationHelper>()
                .BuildServiceProvider();

            var connectionString = "server=127.0.0.1;port=3306;user=root;password=;database=Issue1264_IceCreamParlor";
            services.AddDbContext<Context>(b => b
                .UseInternalServiceProvider(efCoreServiceProvider)
                .UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    b => b.SchemaBehavior(
                            MySqlSchemaBehavior.Translate,
                            (schemaName, objectName) => objectName) // <-- this is the second part that is needed to map
                                                                    //     schemas to databases
                        .CharSetBehavior(CharSetBehavior.NeverAppend))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });

            // HACK to ensure that the databases exists for this sample.
            // Don't use this in production code.
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<Context>();

                var connectionString = context.Database.GetDbConnection().ConnectionString;
                var csb = new MySqlConnectionStringBuilder(connectionString)
                {
                    Database = string.Empty,
                };

                using var connection = new MySqlConnection(csb.ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "DROP DATABASE IF EXISTS `Issue1264_Bakery`;";
                command.ExecuteNonQuery();

                context.Database.EnsureDeleted();

                command.CommandText = "CREATE DATABASE IF NOT EXISTS `Issue1264_Bakery`;";
                command.ExecuteNonQuery();

                context.Database.EnsureCreated();
                
                // Seed some data.
                context.Database.ExecuteSqlInterpolated($"DELETE FROM `Issue1264_IceCreamParlor`.`IceCreams`;");
                context.Database.ExecuteSqlInterpolated($"INSERT INTO `Issue1264_IceCreamParlor`.`IceCreams` (`IceCreamId`, `Name`) VALUES (1, 'Vanilla');");
                context.Database.ExecuteSqlInterpolated($"INSERT INTO `Issue1264_IceCreamParlor`.`IceCreams` (`IceCreamId`, `Name`) VALUES (2, 'Chocolate');");
                
                context.Database.ExecuteSqlInterpolated($"DELETE FROM `Issue1264_Bakery`.`Cookies`;");
                context.Database.ExecuteSqlInterpolated($"INSERT INTO `Issue1264_Bakery`.`Cookies` (`CookieId`, `Name`) VALUES (1, 'Basic');");
                context.Database.ExecuteSqlInterpolated($"INSERT INTO `Issue1264_Bakery`.`Cookies` (`CookieId`, `Name`) VALUES (2, 'Chocolate Chip');");
            }
        }
    }
}