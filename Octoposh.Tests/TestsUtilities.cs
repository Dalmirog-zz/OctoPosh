using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client;

namespace Octoposh.Tests
{
    /// <summary>
    /// Helper class whose purpose is to provie some properties/methods that will aid during unit tests.
    /// </summary>
    static class TestUtilities
    {
        /// <summary>
        /// Gets the path where the tests are currently running
        /// </summary>
        public static string TestsPath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty); }
            set { }
        }

        /// <summary>
        /// exposes an OctopusRepository to use in tests
        /// </summary>
        public static OctopusRepository Repository
        {
            get
            {
                var octopusUrl = string.Concat("http://localhost:", ConfigurationManager.AppSettings["OctopusBindingPort"]);

                //todo do something about this terrible approach
                //var octopusApiKey = ConfigurationManager.AppSettings["OctopusAPIKey"];

                var octopusApiKey = Environment.GetEnvironmentVariable("OctopusAPIKey");

                var endpoint = new OctopusServerEndpoint(octopusUrl, octopusApiKey);

                var repository = new OctopusRepository(endpoint);

                return repository;
            }
            set { }
        }
    }
}
