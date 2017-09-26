using Microsoft.Extensions.Configuration;
using Octopus.Client;
using Octopus.Client.Model;
using System.Threading.Tasks;

namespace Octoposh.TestDataGenerator
{
    public static class OctopusRepository
    {
        /// <summary>
        /// Returns the repository class the tests will use to connect to Octopus
        /// </summary>
        /// <returns>Task&lt;IOctopusAsyncRepository&gt;</returns>
        public static async Task<IOctopusAsyncRepository> GetRepository()
        {

            var config = new ConfigurationBuilder()                
                .AddJsonFile("appConfig.json")
                .Build();

            //getting data to connect to Octopus from appConfig.json
            var username = config["OctopusAdmin"];
            var password = config["OctopusPassword"];
            var octopuswebListenPort = config["OctopuswebListenPort"];


            var serverUrl = "http://localhost:" + octopuswebListenPort;

            var client = await OctopusAsyncClient.Create(new OctopusServerEndpoint(serverUrl));
            
            await client.Repository.Users.SignIn(new LoginCommand { Username = username, Password = password });

            return client.Repository;
        }
    }
}
