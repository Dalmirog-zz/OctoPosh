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
    public class GetOctopusLifecycleTests
    {
        private static readonly string CmdletName = "Get-OctopusLifecycle";
        private static readonly Type CmdletType = typeof(GetOctopusLifecycle);

        [TestCase("LifecycleTests_Lifecycle1")]
        [TestCase("LifecycleTests_Lifecycle2")]
        public void GetLifecycleBySingleName(string lifecycleName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "lifecycleName",
                    SingleValue = lifecycleName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusLifecycle>();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].Name, lifecycleName);
        }

        [TestCase(new[] {"LifecycleTests_Lifecycle1", "LifecycleTests_Lifecycle2"}, null)]
        [TestCase(new[] {"LifecycleTests_Lifecycle2", "Default Lifecycle" }, null)]
        public void GetLifecycleByMultipleNames(string[] lifecycleNames, string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "lifecycleName",
                    MultipleValue = lifecycleNames
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusLifecycle>();

            Assert.AreEqual(2, results.Count);
            var lifecycleNamesFromResults = results.Select(l => l.Name).ToList();

            foreach (var lifecycleName in lifecycleNames)
            {
                Assert.Contains(lifecycleName,lifecycleNamesFromResults);
            }
        }

        [TestCase("Default*")]
        [TestCase("*1")]
        [TestCase("*2")]
        public void GetLifecycleByNameUsingWildcard(string namePattern)
        {
            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "Name",
                    SingleValue = namePattern
                }
            };
            
            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusLifecycle>();

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(pattern.IsMatch(results[0].Name));
        }

        [TestCase("Default Life")]
        [TestCase("Default Bicycle")]
        public void DontGetLifecycleIfNameDoesntMatch(string resourceName)
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusLifecycle>();

            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void GetLifecycleUsingResourceOnlyReturnsRawResource()
        {
            var lifecycleName = "Default Lifecycle";

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                },
                new CmdletParameter()
                {
                    Name = "lifecycleName",
                    SingleValue = lifecycleName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<LifecycleResource>>();

            //If [results] has at least one item, It'll be of the resource type declared on the powershell.Invoke line, meaning the test passed
            Assert.Greater(results[0].Count, 0);
            ;
        }
    }
}
