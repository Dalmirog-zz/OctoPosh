using System;
using System.Collections.Generic;
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
    public class GetOctopusResourceModelTests
    {
        private static readonly string CmdletName = "Get-OctopusResourceModel";
        private static readonly Type CmdletType = typeof(GetOctopusResourceModel);
        
        [Test]
        public void GetEnvironmentModel()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    SingleValue = "Environment"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var results = powershell.Invoke<EnvironmentResource>();

            Assert.AreEqual(results.Count,1);
        }

        [Test]
        public void GetProjectModel()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    SingleValue = "Project"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var results = powershell.Invoke<ProjectResource>();

            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void GetProjectGroupModel()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    SingleValue = "ProjectGroup"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var results = powershell.Invoke<ProjectGroupResource>();

            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void GetNugetFeedModel()
        {
            var synonyms = new string [] {"NugetFeed","ExternalFeed"};

            foreach (var synonym in synonyms)
            {
                var parameters = new List<CmdletParameter>
                {
                    new CmdletParameter()
                    {
                        Name = "Resource",
                        SingleValue = synonym
                    }
                };

                var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

                //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
                var results = powershell.Invoke<NuGetFeedResource>();

                Assert.AreEqual(results.Count, 1);
            }
        }

        [Test]
        public void GetLibraryVariableSetModel()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    SingleValue = "LibraryVariableSet"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var results = powershell.Invoke<LibraryVariableSetResource>();

            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void GetMachineModel()
        {
            var synonyms = new string[] { "Machine", "Target" };

            foreach (var synonym in synonyms)
            {
                var parameters = new List<CmdletParameter>
                {
                    new CmdletParameter()
                    {
                        Name = "Resource",
                        SingleValue = synonym
                    }
                };

                var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

                //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
                var results = powershell.Invoke<MachineResource>();

                Assert.AreEqual(results.Count, 1);
            }
        }

        [Test]
        public void GetLifecycleModel()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    SingleValue = "Lifecycle"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var results = powershell.Invoke<LifecycleResource>();

            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void GetTeamModel()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    SingleValue = "Team"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var results = powershell.Invoke<TeamResource>();

            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void GetChannelModel()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    SingleValue = "Channel"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var results = powershell.Invoke<ChannelResource>();

            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void GetTenantModel()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    SingleValue = "Tenant"
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var results = powershell.Invoke<TenantResource>();

            Assert.AreEqual(results.Count, 1);
        }
    }
}