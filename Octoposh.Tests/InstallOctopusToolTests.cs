using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class InstallOctopusToolTests
    {
        private static readonly string CmdletName = "Install-OctopusTool";
        private static readonly Type CmdletType = typeof(InstallOctopusTool);
        private static readonly string TempDirectory = Path.Combine(TestUtilities.TestsPath, @"TestAssets\Temp");
        private static readonly string NugetRepositoryURL = Path.Combine(TestUtilities.TestsPath, @"TestAssets\Repository");
        private static readonly string Version1 = "1.0.0";

        public InstallOctopusToolTests()
        {
            OctoposhEnvVariables.NugetRepositoryURL = NugetRepositoryURL;
            OctoposhEnvVariables.OctopusToolsFolder = TempDirectory;
        }

        [Test]
        public void InstallToolByVersionAndSetAsDefault()
        {
            ClearTempDirectory();

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

            Assert.DoesNotThrow(() => powershell.Invoke());

            var expectedpath = Path.Combine(TestUtilities.TestsPath, TempDirectory,String.Concat("OctopusTools.", version), "Octo.exe");

            Assert.AreEqual(OctoposhEnvVariables.Octoexe, expectedpath);
        }

        [Test]
        public void InstallToolByLatestAndDoNotSetAsDefault()
        {
            //Clearing temp directory at beginning of the test
            ClearTempDirectory();

            //Setting the OctoExe environment variable to a fake path, to make sure that after running the cmdlet the faka path remains, meaning the recently downloaded Octo.exe wasn't set as default
            var fakeOctoExepath = @"Fake\path";
            OctoposhEnvVariables.Octoexe = fakeOctoExepath;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Latest"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            Assert.DoesNotThrow(() => powershell.Invoke());

            //Asserting that the OctoExe environment variable is still == fakeOctoExePath
            Assert.AreEqual(OctoposhEnvVariables.Octoexe, fakeOctoExepath);
        }

        /// <summary>
        /// Deletes all files from the fixed "Temp" directory that these tests will use to donwload octo.exe.
        /// The actuall path will be a combination of TestUtilities.TestsPath + TempDirectory
        /// </summary>
        private void ClearTempDirectory()
        {
            var fullTempDirPath = Path.Combine(TestUtilities.TestsPath, TempDirectory);

            if (!Directory.Exists(fullTempDirPath))
            {
                Directory.CreateDirectory(fullTempDirPath);
            }

            DirectoryInfo di = new DirectoryInfo(fullTempDirPath);

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