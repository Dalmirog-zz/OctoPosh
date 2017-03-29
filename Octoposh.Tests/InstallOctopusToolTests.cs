using System;
using System.Collections.Generic;
using System.IO;
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
    public class InstallOctopusToolTests
    {
        private static readonly string CmdletName = "Install-OctopusTool";
        private static readonly Type CmdletType = typeof(InstallOctopusTool);
        private static readonly string Repository = @"TestAssets\Repository";
        private static readonly string TempDirectory = Path.Combine(TestsUtilities.TestsPath, @"TestAssets\Temp");
        private static readonly string NugetRepositoryURL = Path.Combine(TestsUtilities.TestsPath,Repository);
        private static readonly string Version1 = "1.0.0";
        private static readonly string LatestVersion = "4.0.0";

        [Test]
        public void InstallToolByVersionAndSetAsDefault()
        {
            ClearTempDirectory();

            OctoposhEnvVariables.NugetRepositoryURL = NugetRepositoryURL;
            OctoposhEnvVariables.OctopusToolsFolder = TempDirectory;

            var version = Version1;
            
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Version",
                    SingleValue = version
                },
                new CmdletParameter()
                {
                    Name = "SetAsDefault"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            powershell.Invoke();
            var expectedpath = Path.Combine(TestsUtilities.TestsPath, TempDirectory,
                String.Concat("OctopusTools.", version), "Octo.exe");

            Assert.AreEqual(OctoposhEnvVariables.Octoexe, expectedpath);
        }

        [Test]
        public void InstallToolByLatestAndDoNotSetAsDefault()
        {
            ClearTempDirectory();

            OctoposhEnvVariables.NugetRepositoryURL = NugetRepositoryURL;
            OctoposhEnvVariables.OctopusToolsFolder = TempDirectory;
            OctoposhEnvVariables.Octoexe = "Whatever";

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Latest"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            powershell.Invoke();

            var expectedpath = Path.Combine(TestsUtilities.TestsPath, TempDirectory,
                String.Concat("OctopusTools.", LatestVersion), "Octo.exe");

            Assert.AreNotEqual(OctoposhEnvVariables.Octoexe, expectedpath);
        }

        private void ClearTempDirectory()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(TestsUtilities.TestsPath, TempDirectory));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
