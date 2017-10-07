using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopusDashboardTests
    {
        private static readonly string CmdletName = "Get-OctopusDashboard";
        private static readonly Type CmdletType = typeof(GetOctopusDashboard);

        [TestCase("DashboardTests_Project1")]
        [TestCase("DashboardTests_Project2")]
        public void GetDashboardBySingleProject(string projectName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);
            foreach (var item in results)
            {
                Assert.AreEqual(item.ProjectName, projectName);
            }
        }

        [TestCase(new[] { "DashboardTests_Project1", "DashboardTests_Project2" },null)]
        [TestCase(new[] { "DashboardTests_Project2", "DeploymentTests_Project1" }, null)]
        public void GetDashboardByMultipleProjects(string[] projectNames,string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Projectname",
                    MultipleValue = projectNames
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            foreach (var projectName in projectNames)
            {
                Assert.Contains(projectName, results.Select(c => c.ProjectName).ToList());//(.Contains(projectName));
            }
        }

        [TestCase("Dev")]
        [TestCase("Stage")]
        [TestCase("Prod")]
        public void GetDashboardBySingleEnvironment(string environmentName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "EnvironmentName",
                    SingleValue = environmentName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);
            foreach (var item in results)
            {
                Assert.AreEqual(item.EnvironmentName,environmentName);
            }
        }


        [TestCase(new[] { "Dev", "Stage" }, null)]
        [TestCase(new[] { "Stage", "Prod" }, null)]
        public void GetDashboardByMultipleEnvironments(string[] environmentNames, string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "EnvironmentName",
                    MultipleValue = environmentNames
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            var environmentsFromResults = results.Select(d => d.EnvironmentName).ToList();

            foreach (var environmentName in environmentNames)
            {
                Assert.Contains(environmentName,environmentsFromResults);
            }
        }

        [TestCase("Success")]
        [TestCase("Failed")]
        [TestCase("Canceled")]
        public void GetDashboardBySingleStatus(string status)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "DeploymentStatus",
                    SingleValue = status
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);
            foreach (var item in results)
            {
                Assert.AreEqual(item.DeploymentStatus, status);
            }
        }


        [TestCase(new[] { "Success", "Failed" }, null)]
        [TestCase(new[] { "Failed", "Canceled" }, null)]
        public void GetDashboardByMultipleStatuses(string[] statuses, string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "DeploymentStatus",
                    MultipleValue = statuses
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);

            var statusFromResults = results.Select(d => d.DeploymentStatus).ToList();

            foreach (var status in statuses)
            {
                Assert.Contains(status,statusFromResults);
            }
        }

        [TestCase("Dev","DashboardTests_Project1")]
        [TestCase("Stage", "DashboardTests_Project1")]
        [TestCase("Prod", "DashboardTests_Project1")]
        [TestCase("Dev", "DashboardTests_Project2")]
        public void GetDashboardByProjectAndEnvironment(string environmentName, string projectName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter() {Name = "EnvironmentName", SingleValue = environmentName},
                new CmdletParameter() {Name = "ProjectName", SingleValue = projectName}
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count,0);
            
            foreach (var item in results)
            {
                Assert.AreEqual(item.ProjectName,projectName);
                Assert.AreEqual(item.EnvironmentName, environmentName);
            }
        }

        [TestCase("DashboardTests_Project1", "Success")]
        [TestCase("DashboardTests_Project1", "Failed")]
        [TestCase("DashboardTests_Project1", "Canceled")]
        [TestCase("DeploymentTests_Project1", "Success")]
        public void GetDashboardByProjectAndStatus(string projectName,string deploymentStatus)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter() {Name = "DeploymentStatus", SingleValue = deploymentStatus},
                new CmdletParameter() {Name = "ProjectName", SingleValue = projectName}
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);

            foreach (var item in results)
            {
                Assert.AreEqual(item.ProjectName, projectName);
                Assert.AreEqual(item.DeploymentStatus, deploymentStatus);
            }
        }

        [TestCase("Dev", "Failed")]
        [TestCase("Dev", "Success")]
        [TestCase("Stage", "Canceled")]
        [TestCase("Stage", "Success")]
        [TestCase("Prod", "Success")]
        public void GetDashboardByEnvironmentAndStatus(string environmentName,string deploymentStatus)
        {
            //var deploymentStatus = "Failed";
            //var environmentName = "Dev";

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter() {Name = "DeploymentStatus", SingleValue = deploymentStatus},
                new CmdletParameter() {Name = "EnvironmentName", SingleValue = environmentName}
            };


            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);

            foreach (var item in results)
            {
                Assert.AreEqual(item.EnvironmentName, environmentName);
                Assert.AreEqual(item.DeploymentStatus, deploymentStatus);
            }
        }

        [Test]
        public void GetDashboardWithoutParametersReturnsValues()
        {
            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);
        }
    }
}
