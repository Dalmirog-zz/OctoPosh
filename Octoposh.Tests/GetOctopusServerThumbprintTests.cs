using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class GetOctopusServerThumbprintTests
    {
        private static readonly string CmdletName = "Get-OctopusServerThumbprint";
        private static readonly Type CmdletType = typeof(GetOctopusServerThumbprint);

        [Test]
        public void GetThumbprint()
        {
            var octopusUrl = string.Concat("http://localhost:", ConfigurationManager.AppSettings["OctopusBindingPort"]);

            Console.WriteLine("Getting thumbprint from Octopus server running on [{0}]", octopusUrl);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType);
            var results = powershell.Invoke<string>();

            Assert.NotNull(results);

            Console.WriteLine("Successfully got the Thumbprint: [{0}] ", results);
        }

    }
}
