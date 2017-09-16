using System;
using System.Threading.Tasks;
using Octoposh.TestDataGenerator.Fixtures;
using Octopus.Client;
using Octopus.Client.Extensions;
using Octopus.Client.Model;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Octoposh.TestDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = "http://localhost:89";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}",theme:AnsiConsoleTheme.Code)
                .CreateLogger();

            var repository = GetRepository(server).Result;

            RunFixtures(repository,
                runConfigurationFixture:true,
                runInfrastructureFixture: true,
                runLibraryFixture:true);

            Console.ReadLine();
        }

        private static async Task<IOctopusAsyncRepository> GetRepository(string serverUrl)
        {
            var client = await OctopusAsyncClient.Create(new OctopusServerEndpoint(serverUrl));
            await client.Repository.Users.SignIn(new LoginCommand { Username = "sa", Password = "Michael2" });
            return client.Repository;
        }

        static void RunFixtures(IOctopusAsyncRepository repository,bool runConfigurationFixture ,bool runInfrastructureFixture, bool runLibraryFixture)
        {
            Log.Information("**Running Fixtures**");

            if (runConfigurationFixture)
            {
                ConfigurationFixture.Run(repository);
            }

            if (runInfrastructureFixture)
            {
                InfrastructureFixture.Run(repository);
            }

            if (runLibraryFixture)
            {
                LibraryFixture.Run(repository);
            }
        }
    }
}
