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
        private static readonly string Project2 = "DeploymentTests_Project2";
        private static readonly string Environment1 = "Dev";
        private static readonly string Environment2 = "Stage";
        private static readonly string Environment3 = "Prod";

        [Test]
        //Testing Single Environment and Project te reduce overhead of having 1 test for project and 1 test for environment.
        public void GetDeploymentBySingleEnvironmentAndProject()
        {
            var environment = "Dev";
            var project = Project1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "EnvironmentName",
                    SingleValue = environment
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = project
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusDeployment>>()[0];

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments",results.Count);
            foreach (var item in results)
            {
                Assert.AreEqual(item.EnvironmentName, environment);
                Assert.AreEqual(item.ProjectName, project);
            }
            Console.WriteLine("The [{0}] deployments found belong to the environment [{1}] on project [{2}]", results.Count,environment,project);
        }

        [Test]
        public void GetDeploymentByMultipleEnvironments()
        {
            var environments = new string[] { Environment2, Environment3};
            var projectName = Project1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "EnvironmentName",
                    MultipleValue = environments
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName //Passing project also to reduce test overall time. Otherwise it'll search on all projects.
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusDeployment>>()[0];

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments", results.Count);

            Assert.IsTrue(results.Any(d => (environments.Contains(d.EnvironmentName)) && (d.ProjectName == projectName)));

            Console.WriteLine("The [{0}] deployments found belong to the environments [{1}] or [{2}] on project [{3}]", results.Count, Environment2, Environment3,projectName);
        }

        [Test]
        public void GetDeploymentByMultipleProjects()
        {
            var environmentName = Environment1;
            var projects = new string[] { Project1, Project2 };

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
                    MultipleValue = projects //Passing project also to reduce test overall time. Otherwise it'll search on all projects.
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusDeployment>>()[0];

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments", results.Count);

            Assert.IsTrue(results.Any(r => (projects.Contains(r.ProjectName)) && (r.EnvironmentName == environmentName)));

            Console.WriteLine("The [{0}] deployments found belong to the environment [{1}] on projects [{2}] or [{3}]", results.Count, environmentName, projects[0],projects[1]);
        }

        [Test]
        public void GetDeploymentBySingleRelease()
        {
            var projectName = Project1;
            var rnd = new Random();

            var releaseVersion = $"0.0.{rnd.Next(1, 12)}";
            
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                },
                new CmdletParameter()
                {
                    Name = "ReleaseVersion",
                    SingleValue = releaseVersion
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusDeployment>>()[0];

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments", results.Count);
            
            Assert.IsTrue(results.Any(d => (d.ReleaseVersion == releaseVersion) && (d.ProjectName == projectName)));
            
            Console.WriteLine("The [{0}] deployments belong to release [{1}] and project [{2}]", results.Count, releaseVersion, projectName);
        }

        [Test]
        public void GetDeploymentByMultipleReleases()
        {
            var projectName = Project1;

            Random rnd = new Random();
            var releaseVersions = new string[]
            {
                string.Format("0.0.{0}", rnd.Next(1, 6)),
                string.Format("0.0.{0}", rnd.Next(7,12))
            }; //Adding 2 different versions from 0.0.1 to 0.0.6 & from 0.0.7 to 0.0.12

            // todo add a CW for each parameter passed in the test
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                },
                new CmdletParameter()
                {
                    Name = "ReleaseVersion",
                    MultipleValue = releaseVersions
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusDeployment>>()[0];

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments", results.Count);
            foreach (var item in results)
            {
                Assert.IsTrue(results.Any(d => (releaseVersions.Contains(d.ReleaseVersion)) && (d.ProjectName == projectName)));
                Assert.IsTrue(releaseVersions.Any(rv => rv.Equals(item.ReleaseVersion)));
                Assert.AreEqual(item.ProjectName, projectName);
            }
            Console.WriteLine("The [{0}] deployments belong to releases [{1}] or [{2}] and project [{3}]", results.Count, releaseVersions[0], releaseVersions[1], projectName);
        }

        [Test]
        public void GetDeploymentByLatestReleases()
        {
            var projectName = Project1;

            var rnd = new Random();
            var latestAmount = rnd.Next(1, 6); //Getting latest 1-6 releases to reduce test time.

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
            var results = powershell.Invoke<List<OutputOctopusDeployment>>()[0];

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments", results.Count);

            var releaseVersions = new HashSet<string>(results.Select(d => d.ReleaseVersion));
            Assert.IsTrue(releaseVersions.Count <= latestAmount);
            Assert.IsTrue(results.Any(d => d.ProjectName == projectName));

            Console.WriteLine("The [{0}] deployments belong to the latest [{1}] releases of project [{2}]", results.Count, latestAmount, projectName);
        }

        [Test]
        public void GetDeploymentAfterDate()
        {
            var baseDate = new DateTime(2017,2,6); //Date of the first deployment on the test data.

            var rnd = new Random();

            var plusDays = rnd.Next(0, 3); //I added test data for aprox 3 days after baseDate. So this will go as high as 3.

            DateTime afterDate = baseDate.AddDays(plusDays); //adding """Random""" amount of days so the test result is not always the same. Good enough for a test. Feel free to PR a better solution :)

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
            var results = powershell.Invoke<List<OutputOctopusDeployment>>()[0];

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments", results.Count);
            var deploymentsStartDates = new HashSet<DateTime>(results.Select(d => d.DeploymentStartTime));

            Assert.IsFalse(deploymentsStartDates.Any(d => afterDate >= d));//Checking afterDate is not greater than the start time of any deployment on the results collection

            Console.WriteLine("The [{0}] deployments found started after [{1}]", results.Count, afterDate);
        }

        //[Test]
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
            var results = powershell.Invoke<List<OutputOctopusDeployment>>()[0];

            Assert.Greater(results.Count, 0);

            Console.WriteLine("Found [{0}] deployments", results.Count);
            var deploymentsStartDates = new HashSet<DateTime>(results.Select(d => d.DeploymentStartTime));

            Assert.IsFalse(deploymentsStartDates.Any(d => beforeDate <= d));//Checking that beforeDate is not lower than the start time of any deployment on the results collection

            Console.WriteLine("The [{0}] deployments found started before [{1}]", results.Count, beforeDate);
        }

        //todo Add test that checks if cmdlet returns package info
    }
}
