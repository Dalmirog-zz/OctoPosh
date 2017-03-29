using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopusToolVersionTests
    {
        private static readonly string CmdletName = "Get-OctopusToolVersion";
        private static readonly Type CmdletType = typeof(GetOctopusToolVersion);
        private static readonly string Version1 = "1.0.0";
        private static readonly string FakeVersion = "9000.0.0";
        private static readonly string HighestVersion = "4.0.0";
        private static readonly string AssetsPath = @"TestAssets\OctopusTools";
        private static readonly string EmptyPath = @"TestAssets\EmptyFolder";
        private static readonly string FakePath = @"AbsolutelyAFakePath\MoreFake";


        [Test]
        public void NotPassingAParameterReturnsMultipleVersions()
        {
            OctoposhEnvVariables.OctopusToolsFolder = Path.Combine(TestsUtilities.TestsPath,AssetsPath);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType);

            var results = powershell.Invoke<List<OctopusToolVersion>>()[0];

            Assert.IsTrue(results.Count > 1);
        }

        [Test]
        public void PassingLatestParameterReturnsHighestVersion()
        {
            OctoposhEnvVariables.OctopusToolsFolder = Path.Combine(TestsUtilities.TestsPath, AssetsPath);

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Latest"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType,parameters);

            var results = powershell.Invoke<List<OctopusToolVersion>>()[0];

            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0].Version == Version.Parse(HighestVersion));
        }

        [Test]
        public void GetToolVersionByVersion()
        {
            OctoposhEnvVariables.OctopusToolsFolder = Path.Combine(TestsUtilities.TestsPath, AssetsPath);
            var version = Version1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Version",
                    SingleValue = version
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<OctopusToolVersion>>()[0];

            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0].Version == Version.Parse(version));
        }

        [Test]
        public void ThrowIfItCantFindSpecificVersion()
        {
            OctoposhEnvVariables.OctopusToolsFolder = Path.Combine(TestsUtilities.TestsPath, AssetsPath);
            var version = FakeVersion;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Version",
                    SingleValue = version
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var ex = Assert.Throws<CmdletInvocationException>(() => powershell.Invoke<List<OctopusToolVersion>>());

            StringAssert.StartsWith("Octo.exe version", ex.Message);
        }

        [Test]
        public void ThrowIfItToolsFolderDoesNotExist()
        {
            OctoposhEnvVariables.OctopusToolsFolder = Path.Combine(TestsUtilities.TestsPath, FakePath);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType);

            var ex = Assert.Throws<CmdletInvocationException>(() => powershell.Invoke<List<OctopusToolVersion>>());

            StringAssert.StartsWith("Could not find a part of the path", ex.Message);
        }

        [Test]
        public void ThrowIfCantFindAnyOctoExeVersionsInFolder()
        {
            OctoposhEnvVariables.OctopusToolsFolder = Path.Combine(TestsUtilities.TestsPath, EmptyPath);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType);
            var ex = Assert.Throws<CmdletInvocationException>(() => powershell.Invoke<List<OctopusToolVersion>>());

            StringAssert.StartsWith("No version of [Octo.exe] was found",ex.Message);
        }

    }
}
