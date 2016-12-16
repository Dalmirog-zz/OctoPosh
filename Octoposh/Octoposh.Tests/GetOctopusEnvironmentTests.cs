using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopuEnvironmentTests
    {
        private static readonly string CmdletName = "Get-OctopusEnvironment";
        private static readonly Type CmdletType = typeof(GetOctopusEnvironment);

        [Test]
        public void GetEnvironmentBySingleName()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "EnvironmentName",
                    SingleValue = "Dev"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType,parameters);
            var results = powershell.Invoke<List<OutputOctopusEnvironment>>();

            Assert.AreEqual(results[0].Count, 1);
            Console.WriteLine("Items Found:");
            foreach (var item in results[0])
            {
                Console.WriteLine(item.Name);
            }
        }

        [Test]
        public void GetEnvironmentByMultipleNames()
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "EnvironmentName", MultipleValue = new[] {"Dev","Stage"}
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusEnvironment>>();

            Assert.AreEqual(results[0].Count, 2);

            Console.WriteLine("Items Found:");
            foreach (var item in results[0])
            {
                Console.WriteLine(item.Name);
            }
        }
    }
}
