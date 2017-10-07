using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopusEnvironmentTests
    {
        private static readonly string CmdletName = "Get-OctopusEnvironment";
        private static readonly Type CmdletType = typeof(GetOctopusEnvironment);

        [TestCase("Dev")]
        [TestCase("Stage")]
        [TestCase("Prod")]
        public void GetEnvironmentBySingleName(string environmentName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Name",
                    SingleValue = environmentName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType,parameters);
            var results = powershell.Invoke<OutputOctopusEnvironment>();

            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(environmentName,results[0].Name);
        }

        [TestCase(new[] { "Dev", "Stage" }, null)]
        [TestCase(new[] { "Stage", "Prod" }, null)]
        public void GetEnvironmentsByMultipleNames(string[] environmentNames,string unused)
        {
            var parameters = new List<CmdletParameter> {
                new CmdletParameter()
                {
                    Name = "Name",
                    MultipleValue = environmentNames
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusEnvironment>();

            Assert.AreEqual(results.Count, 2);
            var environmentNamesFromResults = results.Select(e => e.Name).ToList();

            foreach (var environmentName in environmentNames)
            {
                Assert.Contains(environmentName,environmentNamesFromResults);
            }
        }

        [TestCase("De*")]
        [TestCase("*tag*")]
        [TestCase("*rod")]
        public void GetEnvironmentsByNameUsingWildcard(string namePattern)
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = namePattern
            }};

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusEnvironment>();

            foreach (var item in results)
            {
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
        }

        [TestCase("Debt")]
        [TestCase("Stager")]
        [TestCase("Purrod")]
        public void DontGetEnvironmentsIfNameDoesntMatch(string environmentName)
        {
            //var environmentName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = environmentName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusEnvironment>();

            Assert.AreEqual(results.Count, 0);
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
            Assert.Greater(results.Count, 0);
            ;
        }
    }
}
