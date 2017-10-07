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
    public class GetOctopusProjectGroupTests
    {
        private static readonly string CmdletName = "Get-OctopusProjectGroup";
        private static readonly Type CmdletType = typeof(GetOctopusProjectGroup);

        [TestCase("ProjectGroupTests_ProjectGroup1")]
        [TestCase("ProjectGroupTests_ProjectGroup2")]
        public void GetProjectGroupBySingleName(string projectGroupName)
        {
            var parameters = new List<CmdletParameter> {
                new CmdletParameter()
                {
                    Name = "Name", SingleValue = projectGroupName
                }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProjectGroup>();

            Assert.AreEqual(1,results.Count);
            Assert.AreEqual(projectGroupName,results[0].Name);
        }

        [TestCase(new[] {"ProjectGroupTests_ProjectGroup1", "ProjectGroupTests_ProjectGroup2"}, null)]
        [TestCase(new[] {"ProjectGroupTests_ProjectGroup3", "ProjectGroupTests_ProjectGroup4"}, null)]
        public void GetProjectGroupsByMultipleNames(string[] projectGroupNames,string unused)
        {
            var parameters = new List<CmdletParameter> {
                new CmdletParameter()
                {
                    Name = "Name", MultipleValue = projectGroupNames
                }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProjectGroup>();

            Assert.AreEqual(2,results.Count);

            var projectGroupNamesFromResults = results.Select(pg => pg.Name).ToList();

            foreach (var projectGroupName in projectGroupNames)
            {
                Assert.Contains(projectGroupName,projectGroupNamesFromResults);
            }
        }

        [TestCase("ProjectGroupTests*")]
        [TestCase("*1")]
        [TestCase("ProjectGroupTests*ProjectGroup*")]
        public void GetProjectGroupsByNameUsingWildcard(string namePattern)
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = namePattern
            }};

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProjectGroup>();

            Assert.Greater(results.Count, 0);
            
            foreach (var item in results)
            {
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
        }

        [TestCase("TotallyANameThatYoullNeverPutToAResource")]
        public void DontGetProjectGroupIfNameDoesntMatch(string resourceName)
        {
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
