using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopusResourceModelTests
    {
        private static readonly string CmdletName = "Get-OctopusResourceModel";
        private static readonly Type CmdletType = typeof(GetOctopusResourceModel);

        [TestCase("ProjectGroup", typeof(ProjectGroupResource))]
        [TestCase("Project", typeof(ProjectResource))]
        [TestCase("Environment", typeof(EnvironmentResource))]
        [TestCase("NugetFeed", typeof(NuGetFeedResource))]
        [TestCase("ExternalFeed", typeof(NuGetFeedResource))]
        [TestCase("LibraryVariableSet", typeof(LibraryVariableSetResource))]
        [TestCase("Machine", typeof(MachineResource))]
        [TestCase("Target", typeof(MachineResource))]
        [TestCase("Lifecycle", typeof(LifecycleResource))]
        [TestCase("Team", typeof(TeamResource))]
        [TestCase("Channel", typeof(ChannelResource))]
        [TestCase("Tenant", typeof(TenantResource))]
        public void GetModels(string resourceName, Type resourceType)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    SingleValue = resourceName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<IResource>();

            Assert.IsInstanceOf(resourceType, results.FirstOrDefault());
        }
    }
}