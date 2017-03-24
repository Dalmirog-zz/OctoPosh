using System;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    class GetOctopusToolsFolderTests
    {
        private static readonly string CmdletName = "Get-OctopusToolsFolder";
        private static readonly Type CmdletType = typeof(GetOctopusToolsFolder);

        [Test]
        public void ReturnsRightPath()
        {
            //setting path to a random string
            var randomString = RandomStringGenerator.Generate();
            OctoposhEnvVariables.OctopusToolsFolder = randomString;

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType);
            var result = powershell.Invoke<string>()[0];

            //making sure the cmdlet returns the same random string
            Assert.AreEqual(randomString, result);
        }
    }
}



