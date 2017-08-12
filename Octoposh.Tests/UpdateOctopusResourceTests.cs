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
            var results1 = powershell1.Invoke<ProjectResource>();

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<ProjectResource>();

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
            var results1 = powershell1.Invoke<ProjectGroupResource>();

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<ProjectGroupResource>();

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        [Test]
        public void UpdateEnvironment()
        {
            var unmodifiedName = "unmodified_TestEnvironment";
            var modifiedName = "modified_TestEnvironment";

            var baseresource = Repository.Environments.FindByName(unmodifiedName);

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
            var results1 = powershell1.Invoke<EnvironmentResource>();

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<EnvironmentResource>();

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        [Test]
        public void UpdateTeam()
        {
            var unmodifiedName = "unmodified_TestTeam";
            var modifiedName = "modified_TestTeam";

            var baseresource = Repository.Teams.FindByName(unmodifiedName);

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
            var results1 = powershell1.Invoke<TeamResource>();

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<TeamResource>();

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        [Test]
        public void UpdateChannel()
        {
            var unmodifiedName = "unmodified_TestChannel";
            var modifiedName = "modified_TestChannel";
            var project = Repository.Projects.FindByName("unmodified_TestProject");

            var baseresource = Repository.Channels.FindByName(project,unmodifiedName);

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
            var results1 = powershell1.Invoke<ChannelResource>();

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<ChannelResource>();

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        [Test]
        public void UpdateVariableSet()
        {
            var variableSetName = "VariableSetTests_Library1";
            var unmodifiedValue = "unmodified_VariableValue";
            var modifiedValue = "modified_VariableValue";

            var libraryVariableSet = Repository.LibraryVariableSets.FindByName(variableSetName);
            var baseresource1 = Repository.VariableSets.Get(libraryVariableSet.VariableSetId);

            baseresource1.Variables[0].Value = modifiedValue;

            var parameters1 = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = baseresource1
                }
            };
            //Changing the name from Unmodified_* => Modified_*
            var powershell1 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters1);
            var results1 = powershell1.Invoke<VariableSetResource>();

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Variables[0].Value, modifiedValue);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            var baseresource2 = Repository.VariableSets.Get(libraryVariableSet.VariableSetId);
            baseresource2.Variables[0].Value = unmodifiedValue;

            var parameters2 = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = baseresource2
                }
            };

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters2);
            var results2 = powershell2.Invoke<VariableSetResource>();

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Variables[0].Value, unmodifiedValue);

            Console.WriteLine("Variable in variable set [{2}] value changed from [{0}] to [{1}] and back to [{0}]", unmodifiedValue, modifiedValue, variableSetName);
        }

        [Test]
        public void UpdateTenant()
        {
            var unmodifiedName = "unmodified_TestTenant";
            var modifiedName = "modified_TestTenant";

            var baseresource = Repository.Tenants.FindByName(unmodifiedName);

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
            var results1 = powershell1.Invoke<List<TenantResource>>()[0];

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<List<TenantResource>>()[0];

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        [Test]
        public void UpdateTagSet()
        {
            var unmodifiedName = "Unmodified_TestTagSet";
            var modifiedName = "Modified_TestTagSet";

            var baseresource = Repository.TagSets.FindByName(unmodifiedName);

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
            var results1 = powershell1.Invoke<List<TagSetResource>>()[0];

            Assert.AreEqual(1, results1.Count);
            Assert.AreEqual(modifiedName, results1[0].Name);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<List<TagSetResource>>()[0];

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        [Test]
        public void UpdateLifecycle()
        {
            var unmodifiedName = "unmodified_TestLifecycle";
            var modifiedName = "modified_TestLifecycle";

            var baseresource = Repository.Lifecycles.FindByName(unmodifiedName);

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
            var results1 = powershell1.Invoke<List<LifecycleResource>>()[0];

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<List<LifecycleResource>>()[0];

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        [Test]
        public void UpdateFeed()
        {
            var unmodifiedName = "unmodified_TestFeed";
            var modifiedName = "modified_TestFeed";

            var baseresource = Repository.Feeds.FindByName(unmodifiedName);

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
            var results1 = powershell1.Invoke<FeedResource>();

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<FeedResource>();

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        [Test]
        public void UpdateMachine()
        {
            var unmodifiedName = "unmodified_TestMachine";
            var modifiedName = "modified_TestMachine";

            var baseresource = Repository.Machines.FindByName(unmodifiedName);

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
            var results1 = powershell1.Invoke<List<MachineResource>>()[0];

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<List<MachineResource>>()[0];

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

        //todo add tests for Tag Sets

        [Test]
        public void UpdateRelease()
        {
            var unmodifiedReleaseNotes = "unmodified_ReleaseNotes";
            var modifiedReleaseNotes = "modified_ReleaseNotes";

            var releaseVersion = "0.0.1";
            var project = Repository.Projects.FindByName("ReleaseTests_Project1");
            var baseresource = Repository.Projects.GetReleaseByVersion(project, releaseVersion);

            baseresource.ReleaseNotes = modifiedReleaseNotes;

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
            var results1 = powershell1.Invoke<ReleaseResource>();

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].ReleaseNotes, modifiedReleaseNotes);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.ReleaseNotes= unmodifiedReleaseNotes;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<ReleaseResource>();

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].ReleaseNotes, unmodifiedReleaseNotes);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedReleaseNotes, modifiedReleaseNotes);
        }

        [Test]
        public void UpdateUserRole()
        {
            var unmodifiedName = "unmodified_TestUserRole";
            var modifiedName = "modified_TestUserRole";

            var baseresource = Repository.UserRoles.FindByName(unmodifiedName);

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
            var results1 = powershell1.Invoke<UserRoleResource>();

            Assert.AreEqual(results1.Count, 1);
            Assert.AreEqual(results1[0].Name, modifiedName);

            // Changing the name back from Modified_* => Unmodified_* for the sake of being able to re-run tests without having to reload test data
            baseresource.Name = unmodifiedName;
            parameters[0].Resource = baseresource;

            var powershell2 = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results2 = powershell2.Invoke<UserRoleResource>();

            Assert.AreEqual(results2.Count, 1);
            Assert.AreEqual(results2[0].Name, unmodifiedName);

            Console.WriteLine("Resource name changed from {0} to {1} and back to {0}", unmodifiedName, modifiedName);
        }

    }
}