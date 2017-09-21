using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Octopus.Client;

namespace Octoposh.Tests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void SetApiKey()
        {
            SetApiKeyEnvVariable();
        }

        private static void SetApiKeyEnvVariable()
        {
            var octopusUrl = string.Concat("http://localhost:", ConfigurationManager.AppSettings["OctopusBindingPort"]);
            var octopusUser = ConfigurationManager.AppSettings["OctopusUser"];
            var octopusPassword = ConfigurationManager.AppSettings["OctopusPassword"];

            var repository = new OctopusRepository(new OctopusServerEndpoint(octopusUrl));

            repository.Users.SignIn(octopusUser, octopusPassword, true);

            var user = repository.Users.GetCurrent();

            var apiKeyPurpose = "API Key for Unit tests";

            var existingAPIkey = repository.Users.GetApiKeys(user).FirstOrDefault(ak => ak.Purpose == apiKeyPurpose);

            if (existingAPIkey != null)
            {
                repository.Users.RevokeApiKey(existingAPIkey);
            }

            var octopusApiKey = repository.Users.CreateApiKey(user, apiKeyPurpose).ApiKey;

            Environment.SetEnvironmentVariable("OctopusURL", octopusUrl);
            Environment.SetEnvironmentVariable("OctopusAPIKey", octopusApiKey);
        }
    }
}