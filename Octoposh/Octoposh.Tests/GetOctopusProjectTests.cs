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
    public class GetOctopusProjectTests
    {
        private static readonly string CmdletName = "Get-OctopusProject";
        private static readonly Type CmdletType = typeof(GetOctopusProject);
        private static readonly string Project1 = "ProjectTests_Project1";
        private static readonly string Project2 = "ProjectTests_Project2";

        [Test]
        public void GetProjectBySingleName()
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = Project1
            }};


            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusProject>>();

            Assert.AreEqual(results[0].Count, 1);
            Console.WriteLine("Items Found:");
            foreach (var item in results[0])
            {
                Console.WriteLine(item.Name);
            }
        }

        [Test]
        public void GetProjectsByMultipleNames()
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", MultipleValue = new[] {Project1,Project2}
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusProject>>();

            //todo this test sucks
            Assert.AreEqual(results[0].Count, 2);

            Console.WriteLine("Items Found:");
            foreach (var item in results[0])
            {
                Console.WriteLine(item.Name);
            }
        }

        [Test]
        public void GetProjectsByNameUsingWildcard()
        {
            var namePattern = "*1";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = namePattern
            }};

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusProject>>();

            Assert.Greater(results[0].Count, 0);
            Console.WriteLine("Resources found: {0}", results[0].Count);

            foreach (var item in results[0])
            {
                Console.WriteLine("Resource name: {0}", item.Name);
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
        }

        [Test]
        public void DontGetProjectIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusProject>>();

            Assert.AreEqual(results[0].Count, 0);
        }

        [Test]
        public void GetProjectByProjectGroup()
        {
            var projectGroupName = "TestProjectGroup";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "ProjectGroupName", SingleValue = projectGroupName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusProject>>();

            foreach (var result in results[0])
            {
                Assert.AreEqual(result.ProjectGroupName, projectGroupName);
            }
        }

        [Test]
        public void GetProjectUsingResourceOnlyReturnsRawResource()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<ProjectResource>>();

            //If [results] has at least one item, It'll be of the base resource type meaning the test was successful
            Assert.Greater(results[0].Count, 0);
            ;
        }

    }
}
