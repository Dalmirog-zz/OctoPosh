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

        [Test]
        public void GetReleaseByProject()
        {
            var projectName = "TestProject1";

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusRelease>>()[0];

            Assert.IsTrue(results.Count > 1);
            foreach (var item in results)
            {
                Assert.AreEqual(item.ProjectName,projectName);
            }
            Console.WriteLine("All releases found [{0}] belong to project [{1}]",results.Count,projectName);
        }

        [Test]
        public void GetReleaseBySingleVersion()
        {
            var projectName = "TestProject1";
            var ReleaseVersion = "0.0.1";

            var parameters = new List<CmdletParameter>();

            parameters.Add(new CmdletParameter()
            {
                Name = "ProjectName",
                SingleValue = projectName
            });

            parameters.Add(new CmdletParameter()
            {
                Name = "ReleaseVersion",
                SingleValue = ReleaseVersion
            });

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusRelease>>()[0];

            Assert.IsTrue(results.Count == 1);
            Assert.AreEqual(results.FirstOrDefault().ReleaseVersion, ReleaseVersion);
        }

        [Test]
        public void GetReleaseByMultipleVersions()
        {
            var projectName = "TestProject1";
            var releaseVersions = new string[]{"0.0.1","0.0.2"};

            var parameters = new List<CmdletParameter>();

            parameters.Add(new CmdletParameter()
            {
                Name = "ProjectName",
                SingleValue = projectName
            });

            parameters.Add(new CmdletParameter()
            {
                Name = "ReleaseVersion",
                MultipleValue = releaseVersions
            });

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusRelease>>()[0];

            Assert.IsTrue(results.Count == 2);

            foreach (var release in results)
            {
                Assert.IsTrue(releaseVersions.Contains(release.ReleaseVersion));
            }
        }

        [Test]
        public void GetReleaseUsingLatestX()
        {
            var projectName = "TestProject1";
            var randomMax = 31; //Setting 31 as the max cause in that particular case It'll force Octopus to paginate
            var latest = new Random().Next(1, randomMax);

            var parameters = new List<CmdletParameter>();

            parameters.Add(new CmdletParameter()
            {
                Name = "ProjectName",
                SingleValue = projectName
            });

            parameters.Add(new CmdletParameter()
            {
                Name = "Latest",
                SingleValue = latest.ToString()
            });

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusRelease>>()[0];

            Assert.IsTrue(results.Count == latest);
        }

        [Test]
        public void GetReleaseUsingResourceOnlyReturnsRawResource()
        {
            var projectName = "TestProject1";
            var latest = 1;

            var parameters = new List<CmdletParameter>();

            parameters.Add(new CmdletParameter()
            {
                Name = "ProjectName",
                SingleValue = projectName
            });

            parameters.Add(new CmdletParameter()
            {
                Name = "Latest",
                SingleValue = latest.ToString()
            });

            parameters.Add(new CmdletParameter()
            {
                Name = "resourceOnly"
            });

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<ReleaseResource>>()[0];

            //If [results] has at least one item, It'll be of the base resource type meaning the test was successful. So no need to assert Resource.Type == ExectedType
            Assert.AreEqual(results.Count, latest);
        }
    }
}
