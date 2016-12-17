using System;
using System.Collections.Generic;
using System.Management.Automation;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;
using Octopus.Client.Model;

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
                    Name = "Name",
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
        public void GetEnvironmentsByMultipleNames()
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", MultipleValue = new[] {"Dev","Stage"}
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

        [Test]
        public void GetEnvironmentsByNameUsingWildcard()
        {
            var namePattern = "S*";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = namePattern
            }};

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusEnvironment>>();

            Assert.Greater(results[0].Count, 0);
            Console.WriteLine("Resources found: {0}",results[0].Count);

            foreach (var item in results[0])
            {
                Console.WriteLine("Resource name: {0}",item.Name);
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
        }

        [Test]
        public void DontGetEnvironmentsIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusEnvironment>>();

            Assert.AreEqual(results[0].Count, 0);
        }

        [Test]
        public void GetEnvironmentUsingResourceOnlyReturnsRawResource()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<EnvironmentResource>>();

            //If [results] has at least one item, It'll be of the base resource type meaning the test was successful
            Assert.Greater(results[0].Count, 0);
            ;
        }
    }
}
