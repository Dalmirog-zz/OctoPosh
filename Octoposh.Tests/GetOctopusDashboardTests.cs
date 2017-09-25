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
    public class GetOctopusDashboardTests
    {
        private static readonly string CmdletName = "Get-OctopusDashboard";
        private static readonly Type CmdletType = typeof(GetOctopusDashboard);
        private static readonly string project1 = "DashboardTests_Project1";
        private static readonly string project2 = "DashboardTests_Project2";


        [Test]
        public void GetDashboardBySingleProject()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = project1
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);
            foreach (var item in results)
            {
                Assert.AreEqual(item.ProjectName, project1);
            }
        }

        [Test]
        public void GetDashboardByMultipleProjects()
        {
            var projects = new string[] {project1, project2};

            bool isProject1 = false;
            bool isProject2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Projectname",
                    MultipleValue = projects
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);
            foreach (var item in results)
            {
                if (item.ProjectName == project1)
                {
                    isProject1 = true;
                }
                else if (item.ProjectName == project2)
                {
                    isProject2 = true;
                }
                else
                {
                    Console.WriteLine("Dashboard entry found for wrong project with name [{0}]",item.ProjectName);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isProject1);
            Assert.IsTrue(isProject2);
        }

        [Test]
        public void GetDashboardBySingleEnvironment()
        {
            var environmentName = "Dev";

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

        [Test]
        public void GetDashboardByMultipleEnvironments()
        {
            var environment1 = "Stage";
            var environment2 = "Prod";
            var environments = new string[] { environment1, environment2 };

            bool isEnvironment1 = false;
            bool isEnvironment2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "EnvironmentName",
                    MultipleValue = environments
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDashboardEntry>();

            Assert.Greater(results.Count, 0);
            foreach (var item in results)
            {
                if (item.EnvironmentName == environment1)
                {
                    isEnvironment1 = true;
                }
                else if (item.EnvironmentName == environment2)
                {
                    isEnvironment2 = true;
                }
                else
                {
                    Console.WriteLine("Dashboard entry found for wrong environment with name [{0}]", item.EnvironmentName);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isEnvironment1);
            Assert.IsTrue(isEnvironment2);
        }

        [Test]
        public void GetDashboardBySingleStatus()
        {
            var status = "Success";

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

        [Test]
        public void GetDashboardByMultipleStatuses()
        {
            var status1 = "Failed";
            var status2 = "Canceled";
            var statuses = new string[] { status1, status2 };

            var isStatus1 = false;
            var isStatus2 = false;

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
            foreach (var item in results)
            {
                if (item.DeploymentStatus == status1)
                {
                    isStatus1 = true;
                }
                else if (item.DeploymentStatus == status2)
                {
                    isStatus2 = true;
                }
                else
                {
                    Console.WriteLine("Dashboard entry found for wrong status with name [{0}]", item.DeploymentStatus);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isStatus1);
            Assert.IsTrue(isStatus2);
        }

        [Test]
        public void GetDashboardByProjectAndEnvironment()
        {
            var environmentName = "Dev";
            var projectName = project1;

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

        [Test]
        public void GetDashboardByProjectAndStatus()
        {
            var deploymentStatus = "Failed";
            var projectName = project1;

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

        [Test]
        public void GetDashboardByEnvironmentAndStatus()
        {
            var deploymentStatus = "Failed";
            var environmentName = "Dev";

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
