using System;
using System.CodeDom;
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
using Octopus.Client.Model.Endpoints;

namespace Octoposh.Tests
{
    [TestFixture]
    public class NewAndRemoveOctopusResourceTests
    {
        private static readonly string CreateCmdletName = "New-OctopusResource";
        private static readonly string RemoveCmdletName = "Remove-OctopusResource";
        private static readonly Type CreateCmdletType = typeof(NewOctopusResource);
        private static readonly Type RemoveCmdletType = typeof(RemoveOctopusResource);
        private static readonly string TestResourceName = RandomNameGenerator.Generate();

        [Test]
        public void CreateAndRemoveEnvironment()
        {
            #region EnvironmentCreate

            var resource = new EnvironmentResource { Name = TestResourceName };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<EnvironmentResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }

            #endregion

            #region EnvironmentDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion
        }

        [Test]
        public void CreateAndRemoveProjectGroup()
        {
            #region ProjectGroupCreate
            var resource = new ProjectGroupResource() { Name = TestResourceName };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<ProjectGroupResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }
            #endregion

            #region ProjectGroupDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion
        }

        [Test]
        public void CreateAndRemoveProject()
        {
            #region ProjectCreate

            var projectGroupId = "ProjectGroups-21";
            var lifecycleID = "Lifecycles-21";

            var resource = new ProjectResource()
            {
                Name = TestResourceName,
                ProjectGroupId = projectGroupId,
                LifecycleId = lifecycleID
            };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<ProjectResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }
            #endregion

            #region ProjectDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion
        }

        [Test]
        public void CreateAndRemoveLifecycle()
        {
            #region LifecycleCreate

            var releaseRetention = new RetentionPeriod(1, RetentionUnit.Items);
            var tentacleRetention = new RetentionPeriod(1, RetentionUnit.Items);

            var resource = new LifecycleResource()
            {
                Name = TestResourceName,
                Description = String.Format("Lifecycle {0}", TestResourceName),
                ReleaseRetentionPolicy = releaseRetention,
                TentacleRetentionPolicy = tentacleRetention
            };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<LifecycleResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }
            #endregion

            #region LifecycleDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion
        }

        [Test]
        public void CreateAndRemoveExternalFeed()
        {
            #region FeedCreate
            
            var resource = new NuGetFeedResource()
            {
                Name = TestResourceName,
                FeedUri = "http://whatever.com"
            };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<NuGetFeedResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }
            #endregion

            #region FeedDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion            
        }

        [Test]
        public void CreateAndRemoveLibraryVariableSet()
        {
            #region LibraryVariableSetCreate

            var resource = new LibraryVariableSetResource()
            {
                Name = TestResourceName,
            };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<LibraryVariableSetResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }
            #endregion

            #region LibraryVariableSetDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion            
        }

        /*
        [Test]
        public void CreateAndRemoveUser()
        {
            #region UserCreate

            var resource = new UserResource()
            {
                Username = TestResourceName,
                DisplayName = TestResourceName,
                Password = TestResourceName,
                IsActive = false,
                IsService = false
            };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<UserResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Username, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Username, createResult.GetType());
            }
            #endregion

            #region UserDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Username, createResult.GetType());
            #endregion            
        }
        */

        [Test]
        public void CreateAndRemoveTenant()
        {
            #region TenantCreate

            var resource = new TenantResource()
            {
                Name = TestResourceName,
            };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<TenantResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }
            #endregion

            #region TenantDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion            
        }

        [Test]
        public void CreateAndRemoveTeam()
        {
            #region TeamCreate

            var resource = new TeamResource()
            {
                Name = TestResourceName,
            };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<TeamResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }
            #endregion

            #region TeamDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion            
        }

        [Test]
        public void CreateAndRemoveChannel()
        {
            #region ChannelCreate

            var projectId = "Projects-1";

            var resource = new ChannelResource()
            {
                Name = TestResourceName,
                ProjectId = projectId
            };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<ChannelResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }
            #endregion

            #region TeamDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion            
        }

        [Test]
        public void CreateAndRemoveMachine()
        {
            #region MachineCreate

            var environmentId = "Environments-1";

            var resource = new MachineResource()
            {
                Name = TestResourceName,
                Endpoint = new OfflineDropEndpointResource()
                {
                    ApplicationsDirectory = "SomePath",
                    DropFolderPath = "SomePath",
                    OctopusWorkingDirectory = "SomePath"
                },
                EnvironmentIds = ReferenceCollection.One(environmentId),
                Roles = ReferenceCollection.One("WebServer"),
                
            };

            var createParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = resource
                }
            };

            var createPowershell = new CmdletRunspace().CreatePowershellcmdlet(CreateCmdletName, CreateCmdletType, createParameters);

            //The fact that the line below doesn't throw is enough to prove that the cmdlet returns the expected object type really. Couldn't figure out a way to make the assert around the Powershell.invoke call
            var createResult = createPowershell.Invoke<MachineResource>().FirstOrDefault();

            if (createResult != null)
            {
                Assert.AreEqual(createResult.Name, TestResourceName);
                Console.WriteLine("Created resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            }
            #endregion

            #region MachineDelete
            var removeParameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "Resource",
                    Resource = createResult
                }
            };

            var removePowershell = new CmdletRunspace().CreatePowershellcmdlet(RemoveCmdletName, RemoveCmdletType, removeParameters);

            var removeResult = removePowershell.Invoke<bool>().FirstOrDefault();

            Assert.IsTrue(removeResult);
            Console.WriteLine("Deleted resource [{0}] of type [{1}]", createResult.Name, createResult.GetType());
            #endregion
        }

    }
}