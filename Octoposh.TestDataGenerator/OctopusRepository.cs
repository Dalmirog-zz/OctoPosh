using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octoposh.TestDataGenerator
{
    public static class OctopusRepository
    {
        public static async Task<IOctopusAsyncRepository> GetRepository()
        {
            var serverUrl = "http://localhost:89";

            var client = await OctopusAsyncClient.Create(new OctopusServerEndpoint(serverUrl));
            await client.Repository.Users.SignIn(new LoginCommand { Username = "sa", Password = "Michael2" });
            return client.Repository;
        }
    }
}
