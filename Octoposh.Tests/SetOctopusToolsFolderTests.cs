using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;

namespace Octoposh.Tests
{
    class SetOctopusToolsFolderTests
    {
        private static readonly string CmdletName = "Set-OctopusToolsFolder";
        private static readonly Type CmdletType = typeof(SetOctopusToolsFolder);

        [Test]
        public void SettingPathSetsEnvironmentVariable()
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

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            powershell.Invoke();

            //making sure the cmdlet set the Octopus tools Folder env variable == randomString
            Assert.AreEqual(randomString, OctoposhEnvVariables.OctopusToolsFolder);
        }
    }
}
