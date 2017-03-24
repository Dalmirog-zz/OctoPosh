using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Octoposh.Cmdlets;
using Octoposh.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    class SetOctopusToolPathTests
    {
        private static readonly string CmdletName = "Set-OctopustoolPath";
        private static readonly Type CmdletType = typeof(SetOctopusToolPath);
        private static readonly string OctoexeVersion = "1.0.0";
        private static readonly string AssetsPath = @"TestAssets\OctopusTools";

        [Test]
        public void SetPathByPath()
        {
            //setting path to a random string
            var randomString = RandomStringGenerator.Generate();

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Path",
                    SingleValue = randomString
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType,parameters);

            //Running the cmdlet should set the $env:OctoExe variable value == randomString
            powershell.Invoke();

            //making sure environment variable was set to randomString
            Assert.AreEqual(OctoposhEnvVariables.Octoexe, randomString);
        }

        [Test]
        public void SetPathByVersion()
        {
            OctoposhEnvVariables.OctopusToolsFolder = Path.Combine(TestsUtilities.GetTestsPath, AssetsPath);
            var version = OctoexeVersion;

            var parameters = new List<CmdletParameter>(){
                new CmdletParameter()
                {
                    Name = "Version",
                    SingleValue = version
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //If the cmdlet doesn't throw, its proof enough that the specific Octo.exe version was found and the path was set
            Assert.DoesNotThrow(() => powershell.Invoke());            
        }

        [Test]
        public void SetPathUsingPipelineInput()
        {
            var randomPath = RandomStringGenerator.Generate();

            //This test is trying to proof that the values returned by Get-OctopusToolVersion can be piped into Set-OctopusToolPath
            //Instead of calling Get-OctopusToolVersion, we'll simply create a fake OctopusToolVersion object and pipe that one to Set-OctopusToolPath
            var pipelineObjects = new List<OctopusToolVersion>()
            {
                new OctopusToolVersion()
                {
                    Path = randomPath,
                    Version = Version.Parse("1.0.0")
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType);

            powershell.Invoke(pipelineObjects);

            //Making sure the Environment variable path has the same value a the path of the object sent through the pipeline.
            Assert.AreEqual(randomPath,OctoposhEnvVariables.Octoexe);
        }
    }
}
