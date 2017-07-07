using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopusProjectGroupTests
    {
        private static readonly string CmdletName = "Get-OctopusProjectGroup";
        private static readonly Type CmdletType = typeof(GetOctopusProjectGroup);

        [Test]
        public void GetProjectGroupBySingleName()
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = "TestProjectGroup1"
            }};


            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProjectGroup>();

            Assert.AreEqual(results.Count, 1);
            Console.WriteLine("Items Found:");
            foreach (var item in results)
            {
                Console.WriteLine(item.Name);
            }
        }



        [Test]
        public void GetProjectGroupsByMultipleNames()
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", MultipleValue = new[] {"TestProjectGroup1","TestProjectGroup2"}
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProjectGroup>();

            Assert.AreEqual(results.Count, 2);

            Console.WriteLine("Items Found:");
            foreach (var item in results)
            {
                Console.WriteLine(item.Name);
            }
        }

        [Test]
        public void GetProjectGroupsByNameUsingWildcard()
        {
            var namePattern = "*1";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = namePattern
            }};

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProjectGroup>();

            Assert.Greater(results.Count, 0);
            Console.WriteLine("Resources found: {0}", results.Count);

            foreach (var item in results)
            {
                Console.WriteLine("Resource name: {0}", item.Name);
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
        }

        [Test]
        public void DontGetProjectGroupIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProjectGroup>();

            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void GetProjectGroupUsingResourceOnlyReturnsRawResource()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<ProjectGroupResource>();

            //If [results] has at least one item, It'll be of the base resource type meaning the test was successful
            Assert.Greater(results.Count, 0);
            ;
        }
    }
}
