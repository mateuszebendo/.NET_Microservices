using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScop = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScop.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context)
        {
            Console.WriteLine("--> Attempting to apply migrations...");
            try
            {
                context.Database.Migrate();
                Console.WriteLine("--> Migrations applied.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Could not run migrations: " + ex.Message);
            }
            
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data...");

                context.Platforms.AddRange(
                    new Platform()
                    {
                        Name = "DotNet",
                        Publisher = "Microsoft",
                        Cost = "Free"
                    }, 
                    new Platform()
                    {
                        Name = "Sql Server",
                        Publisher = "Microsoft",
                        Cost = "Free"
                    }, 
                    new Platform()
                    {
                        Name = "Kubernetes",
                        Publisher = "Cloud Native Computing Foundation",
                        Cost = "Free"
                    }
                );

                context.SaveChanges();
                
                Console.WriteLine("--> Seeding data completed.");
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}