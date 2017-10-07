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
    public class GetOctopusReleaseTests
    {
        private static readonly string CmdletName = "Get-OctopusRelease";
        private static readonly Type CmdletType = typeof(GetOctopusRelease);

        [TestCase("ReleaseTests_Project1")]
        [TestCase("ReleaseTests_Project2")]
        public void GetReleaseByProject(string projectName)
        {
            var parameters = new List<CmdletParameter>{

                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusRelease>();

            Assert.Greater(results.Count,1);

            foreach (var item in results)
            {
                Assert.AreEqual(item.ProjectName,projectName);
            }
        }
        
        [TestCase("1.0.0")]
        [TestCase("2.0.0")]
        [TestCase("3.0.0")]
        public void GetReleaseBySingleVersion(string releaseVersion)
        {
            var projectName = "ReleaseTests_Project1";

            var parameters = new List<CmdletParameter>()
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
            var results = powershell.Invoke<OutputOctopusRelease>();

            Assert.IsTrue(results.Count == 1);
            Assert.AreEqual(results[0].ReleaseVersion, releaseVersion);
        }

        [TestCase(new[] { "1.0.0", "2.0.0" }, null)]
        [TestCase(new[] { "3.0.0", "4.0.0","5.0.0" }, null)]
        [TestCase(new[] { "6.0.0", "7.0.0", "8.0.0","9.0.0" }, null)]
        public void GetReleaseByMultipleVersions(string[] releaseVersions, string unused)
        {
            var projectName = "ReleaseTests_Project1";

            var parameters = new List<CmdletParameter>()
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
            var results = powershell.Invoke<OutputOctopusRelease>();

            Assert.IsTrue(results.Count == releaseVersions.Length);

            var releaseNumbersInResults = results.Select(r => r.ReleaseVersion).ToList();

            foreach (var releaseVersion in releaseVersions)
            {
                Assert.Contains(releaseVersion,releaseNumbersInResults);
            }
        }

        [TestCase("1.0.0", "1.0..")]
        [TestCase("2.0.0", "one.zero.zero")]
        public void GetReleaseByMultipleVersionsWithUnexisting(string goodVersion,string badVersion)
        {
            var projectName = "ReleaseTests_Project1";

            var releaseVersions = new[] { goodVersion, badVersion };

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
            var results = powershell.Invoke<OutputOctopusRelease>();

            Assert.IsTrue(results.Count == 1);

            Assert.IsTrue(results[0].ProjectName == projectName);
            Assert.AreEqual(results[0].ReleaseVersion,goodVersion);
        }

        [TestCase(3)]
        [TestCase(5)]
        [TestCase(31)]
        
        public void GetReleaseUsingLatestX(int randomMax)
        {
            var projectName = "ReleaseTests_Project1";
            var latest = new Random().Next(1, randomMax); //The test will get the latest X releases where X is a random number between 1 and randomMax

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                },
                new CmdletParameter()
                {
                    Name = "Latest",
                    SingleValue = latest.ToString()
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<OutputOctopusRelease>();

            Assert.IsTrue(results.Count == latest);

            Assert.That(results.Any(r => r.ProjectName == projectName));
        }

        [Test]
        public void GetReleaseUsingResourceOnlyReturnsRawResource()
        {
            var projectName = "ReleaseTests_Project1";
            var latest = 1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                },
                new CmdletParameter()
                {
                    Name = "Latest",
                    SingleValue = latest.ToString()
                },
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<ReleaseResource>>()[0];

            //If [results] has at least one item, It'll be of the base resource type meaning the test was successful. So no need to assert Resource.Type == ExectedType
            Assert.AreEqual(results.Count, latest);
        }
    }
}
