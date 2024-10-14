using System;
using System.Collections.Generic;
using System.Linq;
using CommandsService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
    public class PrepDb
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
                    new Platform
                    {
                        ExternalId = 1,
                        Name = "DotNet",
                        Commands = new List<Command>
                        {
                            new Command { HowTo = "Criar um novo aplicativo console", CommandLine = "dotnet new console" },
                            new Command { HowTo = "Construir o projeto", CommandLine = "dotnet build" }
                        }
                    },
                    new Platform
                    {
                        ExternalId = 2,
                        Name = "Sql Server",
                        Commands = new List<Command>
                        {
                            new Command { HowTo = "Conectar ao banco de dados", CommandLine = "sqlcmd -S servidor -U usuário -P senha" },
                            new Command { HowTo = "Restaurar o banco de dados", CommandLine = "RESTORE DATABASE [nome_bd] FROM DISK = 'caminho'" }
                        }
                    },
                    new Platform
                    {
                        ExternalId = 3,
                        Name = "Kubernetes",
                        Commands = new List<Command>
                        {
                            new Command { HowTo = "Aplicar uma configuração", CommandLine = "kubectl apply -f config.yaml" },
                            new Command { HowTo = "Listar pods", CommandLine = "kubectl get pods" }
                        }
                    },
                    new Platform
                    {
                        ExternalId = 4,
                        Name = "Docker",
                        Commands = new List<Command>
                        {
                            new Command { HowTo = "Construir uma imagem", CommandLine = "docker build -t nome_imagem ." },
                            new Command { HowTo = "Executar um contêiner", CommandLine = "docker run -p 80:80 nome_imagem" }
                        }
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