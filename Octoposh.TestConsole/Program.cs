using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client;
using Octopus.Client.Model;
namespace Octoposh.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpoint = new OctopusServerEndpoint("http://localhost:82", "API-B3ZK7BTFAKSKRTCHQFKAZNPT5Y");
            var repository = new OctopusRepository(endpoint);
        }
    }
}
