using System;
using System.Collections.Generic;
using System.Configuration;
using System.Management.Automation;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;
using Octopus.Client.Model;
using Octopus.Client;

namespace Octoposh.Tests
{
    [TestFixture]
    public class UpdateOctopusResourceTests
    {
        private static readonly string CmdletName = "Update-OctopusResource";
        private static readonly Type CmdletType = typeof(UpdateOctopusResource);
        private static readonly string OctopusUrl = string.Concat("http://localhost:", ConfigurationManager.AppSettings["OctopusBindingPort"]);
        private static readonly string OctopusApiKey = ConfigurationManager.AppSettings["OctopusAPIKey"];
        private static readonly OctopusRepository Repository = new OctopusRepository(new OctopusServerEndpoint(OctopusUrl,OctopusApiKey));

        [Test]
        public void UpdateProject()
        {
            var unmodifiedName = "unmodified_TestProject";
            var modifiedName = "modified_Testproject";

            var baseresource = Repository.Projects.FindByName(unmodifiedName);

            baseresource.Name = modifiedName;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = baseresource
                }
            };
            //Changing the name from Unmodified_* => Modified_*
            var powershell1 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results1 = powershell1.Invoke<List<ProjectResource>>()[0];

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<List<ProjectResource>>()[0];

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        [Test]
        public void UpdateProjectGroup()
        {
            var unmodifiedName = "unmodified_TestProjectGroup";
            var modifiedName = "modified_TestprojectGroup";

            var baseresource = Repository.ProjectGroups.FindByName(unmodifiedName);

            baseresource.Name = modifiedName;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = baseresource
                }
            };
            //Changing the name from Unmodified_* => Modified_*
            var powershell1 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results1 = powershell1.Invoke<List<ProjectGroupResource>>()[0];

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<List<ProjectGroupResource>>()[0];

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }
    }
}