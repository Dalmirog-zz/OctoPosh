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

        [TestCase("ProjectTests_Project1")]
        [TestCase("ProjectTests_Project2")]
        public void GetProjectBySingleName(string projectName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Name", SingleValue = projectName
                }
                
            };


            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProject>();

            Assert.AreEqual(1,results.Count);
            Assert.AreEqual(projectName,results[0].Name);
        }

        [TestCase(new[] {"ProjectTests_Project1","ProjectTests_Project2"},null)]
        [TestCase(new[] { "ProjectTests_Project3", "ProjectTests_Project4" }, null)]
        public void GetProjectsByMultipleNames(string[] projectNames,string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Name",
                    MultipleValue = projectNames
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProject>();

            Assert.AreEqual(2,results.Count);

            var projectNamesInResults = results.Select(p => p.Name).ToList();
            
            foreach (var projectName in projectNames)
            {
                Assert.Contains(projectName,projectNamesInResults);
            }
        }

        [TestCase("ProjectTests*")]
        [TestCase("ProjectTests*_Project*")]
        [TestCase("*1")]
        public void GetProjectsByNameUsingWildcard(string namePattern)
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = namePattern
            }};

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProject>();

            Assert.Greater(results.Count, 0);
            
            foreach (var item in results)
            {
                Console.WriteLine("Resource name: {0}", item.Name);
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
        }

        [TestCase("TotallyANameThatYoullNeverPutToAResource")]
        public void DontGetProjectIfNameDoesntMatch(string projectName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Name", SingleValue = projectName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProject>();

            Assert.AreEqual(0,results.Count);
        }

        [TestCase("DashboardTests_ProjectGroup")]
        [TestCase("DeploymentTests_ProjectGroup")]
        [TestCase("ProjectTests_ProjectGroup")]
        public void GetProjectByProjectGroup(string projectGroupName)
        {
            //var projectGroupName = "TestProjectGroup";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "ProjectGroupName", SingleValue = projectGroupName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProject>();

            Assert.Greater(results.Count,0);

            foreach (var project in results)
            {
                Assert.AreEqual(projectGroupName,project.ProjectGroupName);
            }
            
        }

        [TestCase(new[] { "DashboardTests_ProjectGroup", "DeploymentTests_ProjectGroup" }, null)]
        [TestCase(new[] { "ProjectTests_ProjectGroup", "ReleaseTests_ProjectGroup" }, null)]
        public void GetProjectsByMultipleProjectGroups(string[] projectGroupNames, string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectGroupName",
                    MultipleValue = projectGroupNames
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusProject>();

            Assert.Greater(results.Count,2);

            var projectGroupNamesInResults = results.Select(p => p.ProjectGroupName).ToList();

            foreach (var projectGroupName in projectGroupNames)
            {
                Assert.Contains(projectGroupName, projectGroupNamesInResults);
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
            Assert.Greater(results.Count, 0);
            ;
        }

    }
}
