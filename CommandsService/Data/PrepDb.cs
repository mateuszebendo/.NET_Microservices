using System;
using System.Collections.Generic;
using System.Linq;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(),
                         serviceScope.ServiceProvider.GetService<IPlatformDataClient>().ReturnAllPlatforms(),
                         serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms, AppDbContext context)
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
            
            /*Console.WriteLine("--> Deleting existing data...");

            if (context.Commands.Any())
            {
                context.Commands.RemoveRange(context.Commands);
            }

            if (context.Platforms.Any())
            {
                context.Platforms.RemoveRange(context.Platforms);
            }

            context.SaveChanges();

            Console.WriteLine("--> Existing data deleted.");*/
            
            Console.WriteLine("--> Seeding new platforms...");

            foreach (var plat in platforms)
            {
                if (!repo.ExternalPlatformExists(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                }
            }

            context.SaveChanges();
            
            Console.WriteLine("--> Seeding data completed.");
            }
    }
}