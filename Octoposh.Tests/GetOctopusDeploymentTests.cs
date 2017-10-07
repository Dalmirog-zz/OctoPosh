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
    public class GetOctopusDeploymentTests
    {
        private static readonly string CmdletName = "Get-OctopusDeployment";
        private static readonly Type CmdletType = typeof(GetOctopusDeployment);
        private static readonly string Project1 = "DeploymentTests_Project1";

        [TestCase("Dev", "DeploymentTests_Project1")]
        [TestCase("Stage", "DeploymentTests_Project1")]
        [TestCase("Stage", "DeploymentTests_Project2")]
        [TestCase("Prod", "DeploymentTests_Project2")]
        public void GetDeploymentBySingleEnvironmentAndProject(string environmentName, string projectName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "EnvironmentName",
                    SingleValue = environmentName
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDeployment>();

            Assert.Greater(results.Count, 0);
            
            foreach (var item in results)
            {
                Assert.AreEqual(item.EnvironmentName, environmentName);
                Assert.AreEqual(item.ProjectName, projectName);
            }
        }

        [TestCase(new[] { "Dev", "Stage" }, "DeploymentTests_Project2")]
        [TestCase(new[] { "Stage", "Prod" }, "DeploymentTests_Project1")]
        public void GetDeploymentByMultipleEnvironments(string[] environmentNames, string projectName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "EnvironmentName",
                    MultipleValue = environmentNames
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName //Passing project also to reduce test overall time. Otherwise it'll search on all projects.
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDeployment>();

            Assert.Greater(results.Count, 0);
            
            var environmentsFromResults = results.Select(d => d.EnvironmentName).ToList();

            foreach (var environmentName in environmentNames)
            {
                Assert.Contains(environmentName,environmentsFromResults);

                foreach (var deployment in results)
                {
                    Assert.AreEqual(projectName,deployment.ProjectName);
                }
            }
        }

        [TestCase(new[] { "DashboardTests_Project1", "DashboardTests_Project2" }, null)]
        [TestCase(new[] { "DashboardTests_Project2", "DeploymentTests_Project1" }, null)]
        public void GetDeploymentByMultipleProjects(string[] projectNames, string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    MultipleValue = projectNames 
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDeployment>();

            Assert.Greater(results.Count, 0);

            var projectsFromResults = results.Select(d => d.ProjectName).ToList();

            foreach (var projectName in projectNames)
            {
                Assert.Contains(projectName,projectsFromResults);
            }
        }

        [TestCase("1.0.0")]
        [TestCase("2.0.0")]
        [TestCase("3.0.0")]
        public void GetDeploymentBySingleRelease(string releaseVersion)
        {
            var projectName = "DeploymentTests_Project1";
            
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName //Passing project also to reduce test overall time. Otherwise it'll search on all projects.
                },
                new CmdletParameter()
                {
                    Name = "ReleaseVersion",
                    SingleValue = releaseVersion
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDeployment>();

            Assert.Greater(results.Count, 0);

            foreach (var deployment in results)
            {
                Assert.AreEqual(releaseVersion, deployment.ReleaseVersion);
                Assert.AreEqual(projectName, deployment.ProjectName);
            }
        }

        [TestCase(new[] { "1.0.0", "2.0.0" }, null)]
        [TestCase(new[] { "3.0.0", "4.0.0" }, null)]
        public void GetDeploymentByMultipleReleases(string[] releaseVersions,string unused)
        {
            var projectName = "DeploymentTests_Project1";

            Random rnd = new Random();
           
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName //Passing project also to reduce test overall time. Otherwise it'll search on all projects.
                },
                new CmdletParameter()
                {
                    Name = "ReleaseVersion",
                    MultipleValue = releaseVersions
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDeployment>();

            var releasesOfResults = results.Select(d => d.ReleaseVersion).ToList();

            foreach (var releaseVersion in releaseVersions)
            {
                Assert.Contains(releaseVersion,releasesOfResults);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetDeploymentByLatestReleases(int latestAmount)
        {
            var projectName = "DeploymentTests_Project1";

            var projectResource = TestUtilities.Repository.Projects.FindByName(projectName);
            var allReleases = TestUtilities.Repository.Projects.GetAllReleases(projectResource);

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                },
                new CmdletParameter()
                {
                    Name = "LatestReleases",
                    SingleValue = latestAmount.ToString()
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDeployment>();

            Assert.Greater(results.Count, 0);

            var versionOfActualLatestNReleases = allReleases.Reverse().Skip(allReleases.Count - latestAmount).Select(r => r.Version).ToList();

            foreach (var deployment in results)
            {
                Assert.Contains(deployment.ReleaseVersion,versionOfActualLatestNReleases);
            }
            
        }

        [Ignore("Need to review")]
        [Test]
        public void GetDeploymentAfterDate()
        {
            var baseDate = new DateTime(2017,2,6); //Date of the first deployment on the test data.

            var rnd = new Random();

            var plusDays = rnd.Next(0, 3); 

            var afterDate = baseDate.AddDays(plusDays); 

            var project = Project1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = project
                },
                new CmdletParameter()
                {
                    Name = "After",
                    SingleValue = afterDate.ToString()
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDeployment>();

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments", results.Count);
            var deploymentsStartDates = new HashSet<DateTime>(results.Select(d => d.DeploymentStartTime));

            Assert.IsFalse(deploymentsStartDates.Any(d => afterDate >= d));

            Console.WriteLine("The [{0}] deployments found started after [{1}]", results.Count, afterDate);
        }

        //todo fix these tests - Need to figure out a way to generate test data to test this.
        [Ignore("Need to review")]
        [Test]
        public void GetDeploymentBeforeDate()
        {
            DateTime baseDate = new DateTime(2017, 2, 11); //Date of the first deployment on the test data.

            DateTime now = DateTime.Now;

            int datesDiff = Convert.ToInt32((now - baseDate).TotalDays); //Diff of days between now and the date of the first deployment to use as MAX value in random

            Random rnd = new Random();
            var minusDays = rnd.Next(1, datesDiff); //getting random number of dats to substract from NOW

            DateTime beforeDate = now.AddDays(-minusDays);

            var project = Project1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = project
                },
                new CmdletParameter()
                {
                    Name = "Before",
                    SingleValue = beforeDate.ToString()
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusDeployment>();

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments", results.Count);
            var deploymentsStartDates = new HashSet<DateTime>(results.Select(d => d.DeploymentStartTime));

            Assert.IsFalse(deploymentsStartDates.Any(d => beforeDate <= d));//Checking that beforeDate is not lower than the start time of any deployment on the results collection

            Console.WriteLine("The [{0}] deployments found started before [{1}]", results.Count, beforeDate);
        }

        //todo Add test that checks if cmdlet returns package info
    }
}
