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
    public class GetOctopuReleaseTests
    {
        private static readonly string CmdletName = "Get-OctopusRelease";
        private static readonly Type CmdletType = typeof(GetOctopusRelease);
        private static readonly string Project1 = "ProjectTests_Project1";

        [Test]
        public void GetReleaseByProject()
        {
            var projectName = Project1;

            var parameters = new List<CmdletParameter>{

                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusRelease>>()[0];

            Console.WriteLine("Found [{0}] releases",results.Count);
            Assert.Greater(results.Count,1);
            foreach (var item in results)
            {
                Assert.AreEqual(item.ProjectName,projectName);
            }
            Console.WriteLine("All releases found [{0}] belong to project [{1}]",results.Count,projectName);
        }

        [Test]
        public void GetReleaseBySingleVersion()
        {
            var projectName = Project1;
            var ReleaseVersion = "0.0.1";

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
                    SingleValue = ReleaseVersion
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusRelease>>()[0];

            Assert.IsTrue(results.Count == 1);
            Assert.AreEqual(results[0].ReleaseVersion, ReleaseVersion);
        }

        [Test]
        public void GetReleaseByMultipleVersions()
        {
            var projectName = Project1;
            var releaseVersions = new string[]{"0.0.1","0.0.2"};

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
            var results = powershell.Invoke<List<OutputOctopusRelease>>()[0];

            Console.WriteLine("Found [{0}] Releases",results.Count);
            Assert.IsTrue(results.Count == 2);

            foreach (var release in results)
            {
                Assert.IsTrue(releaseVersions.Contains(release.ReleaseVersion));
            }

            Console.WriteLine("The [{0}] releases have the version numbers [{1}] and [{2}]",results.Count,releaseVersions[0],releaseVersions[1]);
        }

        [Test]
        public void GetReleaseByMultipleVersionsWithUnexisting()
        {
            var projectName = Project1;

            var goodVersion = "0.0.1";
            var badVersion = "whatever";

            var releaseVersions = new string[] { goodVersion, badVersion };

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

            Console.WriteLine("Looking for releases with version numbers [{0}] and [{1}]. The test expects to find only 1 release with version [{1}] for the project [{2}]",badVersion,goodVersion,projectName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusRelease>>()[0];

            Console.WriteLine("Found [{0}] releases", results.Count);
            Assert.IsTrue(results.Count == 1);

            Assert.AreEqual(results[0].ReleaseVersion,goodVersion);
            Assert.IsTrue(results[0].ProjectName == projectName);
            Console.WriteLine("The release found has the version [{0}] and belongs to the project [{1}]",results[0].ReleaseVersion,projectName);
        }

        [Test]
        public void GetReleaseUsingLatestX()
        {
            var projectName = Project1;
            var randomMax = 31; //Setting 31 as the max cause in that particular case It'll force Octopus to paginate
            var latest = new Random().Next(1, randomMax);

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

            Console.WriteLine("Looking for the last [{0}] releases for project [{1}]", latest, projectName);
            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<OutputOctopusRelease>>()[0];

            Console.WriteLine("Found [{0}] releases", results.Count);
            Assert.IsTrue(results.Count == latest);

            Assert.That(results.Any(r => r.ProjectName == projectName));
            Console.WriteLine("All releases found belong to the project [{0}]",projectName);
        }

        [Test]
        public void GetReleaseUsingResourceOnlyReturnsRawResource()
        {
            var projectName = Project1;
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
