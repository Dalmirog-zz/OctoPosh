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
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}",theme:AnsiConsoleTheme.Code)
                .CreateLogger();

            RunFixtures(
                runConfigurationFixture:true,
                runLibraryFixture: true,
                runInfrastructureFixture: true,
                runDeploymentFixture:true);

            Console.ReadLine();
        }

        static void RunFixtures(bool runConfigurationFixture , bool runLibraryFixture, bool runInfrastructureFixture, bool runDeploymentFixture)
        {
            Log.Information("**Running Fixtures**");

            if (runConfigurationFixture)
            {
                ConfigurationFixture.Run();
            }

            if (runLibraryFixture)
            {
                LibraryFixture.Run();
            }

            if (runInfrastructureFixture)
            {
                InfrastructureFixture.Run();
            }

            if (runDeploymentFixture)
            {
                DeploymentFixture.Run();
            }
        }
    }
}
