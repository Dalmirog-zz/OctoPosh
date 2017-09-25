using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Model.Endpoints;
using Serilog;

namespace Octoposh.TestDataGenerator.Fixtures
{
    static class InfrastructureFixture
    {
        private static IOctopusAsyncRepository _repository;

        #region ResourcesToCreate
        private static readonly string[] EnvironmentNames = { "Dev", "Stage", "Prod", "Unmodified_TestEnvironment", "MachineTests_Environment" };
        private static readonly string[] LifecycleNames = { "Default Lifecycle", "LifecycleTests_Lifecycle1", "LifecycleTests_Lifecycle2", "unmodified_TestLifecycle" };

        private static readonly string[] ProjectGroupNames = { "DashboardTests_ProjectGroup", "DeploymentTests_ProjectGroup", "ReleaseTests_ProjectGroup", "TestProjectGroup1", "TestProjectGroup2", "unmodified_TestProjectGroup", "ProjectGroupTests_ProjectGroup", "ProjectGroupTests_ProjectGroup2", "ProjectTests_ProjectGroup", "ReleaseTests_ProjectGroup" };
        private static readonly string[] ProjectNames = { "DashboardTests_Project1", "DashboardTests_Project2", "DeploymentTests_Project1", "DeploymentTests_Project2", "ProjectTests_Project1", "ProjectTests_Project2", "ReleaseTests_Project1", "unmodified_TestProject" };

        private static readonly string[] TagSetNames = { "TagSetTests_TagSet1", "TagSetTests_TagSet2", "unmodified_TagSet" };
        private static readonly string[] TenantNames = { "TenantTests_Tenant1", "TenantTests_Tenant2", "unmodified_Tenant" };
        #endregion

        public static void Run()
        {
            _repository = OctopusRepository.GetRepository().Result;

            Log.Logger.Information("**Running Infrastructure Fixture**");

            var pgs = CreateProjectGroups();

            CreateLifecycles();
            CreateProjects(pgs);
            CreateLifecycles();
            CreateTagSets();
            CreateTenants();

            Log.Logger.Information("**Finished running Infrastructure Fixture**");
        }

        #region Projects/Project Groups/Channels

        private static List<ProjectGroupResource> CreateProjectGroups()
        {
            var projectGroups = new List<ProjectGroupResource>();

            foreach (var projectGroupName in ProjectGroupNames)
            {
                var projectGroup = _repository.ProjectGroups.FindByName(projectGroupName).Result ??
                                   new ProjectGroupResource();

                projectGroup.Name = projectGroupName;
                projectGroup.Description = GeneralResourceProperty.ResourceDescription;

                try
                {
                    Log.Information($"Creating/Modifying Project Group [{projectGroup.Name}]");
                    projectGroups.Add(projectGroup.Id == null ?
                        _repository.ProjectGroups.Create(projectGroup).Result :
                        _repository.ProjectGroups.Modify(projectGroup).Result
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return projectGroups;
        }

        private static void CreateProjects(List<ProjectGroupResource> projectGroups)
        {
            var projectList = new List<ProjectResource>();
            var lifecycle = _repository.Lifecycles.FindByName("Default Lifecycle").Result;// All projects will use the same lifecycle
            var libraryVariableset = _repository.LibraryVariableSets.FindByName("VariableSetTests_Library1").Result;//All projects will use the same LVS.

            foreach (var projectName in ProjectNames)
            {
                //projects will be assigned to projectgroups that have the same prefix. i.e DashboardTests_Project => DashboardTests_ProjectGroup
                var projectNamePrefix = projectName.Split('_')[0];
                
                var projectGroup = projectGroups.First(pg => pg.Name.StartsWith(projectNamePrefix));

                var project = _repository.Projects.FindByName(projectName).Result ?? new ProjectResource();

                project.Name = projectName;
                project.Description = GeneralResourceProperty.ResourceDescription;
                project.LifecycleId = lifecycle.Id;
                project.ProjectGroupId = projectGroup.Id;
                project.TenantedDeploymentMode = TenantedDeploymentMode.TenantedOrUntenanted;
                project.IncludedLibraryVariableSetIds.Add(libraryVariableset.Id);

                try
                {
                    Log.Information($"Creating/Modifying Project [{project.Name}]");

                    project = project.Id == null
                        ? _repository.Projects.Create(project).Result
                        : _repository.Projects.Modify(project).Result;

                    projectList.Add(project);

                    //Adding steps to the process. All pre-existing steps are deleted and the default ones added during this process.
                    var deploymentProcess = UpdateDeploymentprocess(project);

                    //Adding extra config to specific the projects
                    switch (project.Name)
                    {
                        case "ProjectTests_Project1":
                            AddChannel("ChannelTests_Channel1", project);
                            AddChannel("ChannelTests_Channel2", project);

                            AddVariableToProject("VariableSetTests_Variable1", "bar",project,new ScopeSpecification()
                            {
                                {ScopeField.Environment,new ScopeValue(_repository.Environments.FindAll().Result.Select(x => x.Id).ToList())},
                                {ScopeField.Channel,new ScopeValue(_repository.Projects.GetChannels(project).Result.Items.Select(x => x.Id).ToList())},
                                {ScopeField.Role,new ScopeValue(_repository.MachineRoles.GetAllRoleNames().Result)},
                                {ScopeField.Action, new ScopeValue(deploymentProcess.Steps.Select(x => x.Actions[0].Id))},
                                {ScopeField.Machine,new ScopeValue(_repository.Machines.FindAll().Result.Select(x => x.Id))}
                            });

                            break;
                        case "ProjectTests_Project2":
                            AddChannel("ChannelTests_Channel1", project);
                            AddChannel("ChannelTests_Channel2", project);
                            break;
                        case "unmodified_TestProject":
                            AddChannel("unmodified_TestChannel", project);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }    
        }

        private static DeploymentProcessResource UpdateDeploymentprocess(ProjectResource project)
        {
            var process = _repository.DeploymentProcesses.Get(project.DeploymentProcessId).Result;

            process.ClearSteps();

            process.Steps.Add(StepLibrary.GetSimpleScriptStep());

            Log.Information($"Modifying deployment proces o project [{project.Name}]");
            process = _repository.DeploymentProcesses.Modify(process).Result;

            return process;
        }

        private static void AddChannel(string channelName,ProjectResource project)
        {
            var channel = _repository.Channels.FindByName(project, channelName).Result ?? new ChannelResource();

            channel.Name = channelName;
            channel.Description = GeneralResourceProperty.ResourceDescription;
            channel.ProjectId = project.Id;
            channel.Rules = new List<ChannelVersionRuleResource>();
            channel.IsDefault = false;

            try
            {
                Log.Information($"Creating/Modifying Channel [{channel.Name}] for project [{project.Name}]");
                if (channel.Id != null)
                    _repository.Channels.Modify(channel);
                else
                    _repository.Channels.Create(channel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void AddVariableToProject(string variableName, string variableValue ,ProjectResource project, ScopeSpecification scope)
        {
            var variableSet = _repository.VariableSets.Get(project.VariableSetId).Result;
            
            variableSet.AddOrUpdateVariableValue(variableName, variableValue, scope);

            Log.Information($"Adding variable [{variableName}] to project [{project.Name}]");
            _repository.VariableSets.Modify(variableSet).Wait();
        }

        #endregion

        #region Environments
        private static void CreateEnvironments()
        {
            foreach (var environmentName in EnvironmentNames)
            {
                var environment = _repository.Environments.CreateOrModify(environmentName,GeneralResourceProperty.ResourceDescription).Result.Instance;
                if (environment != null)
                {
                    Log.Logger.Information($"Creating Environment [{environment.Name}]");

                    switch (environment.Name)
                    {
                        case "Unmodified_TestEnvironment":
                            CreateMachine(environment, MachineType.CloudRegion, environment.Name + "_CloudRegion");
                            break;
                        case "MachineTests_Environment":
                            CreateMachine(environment, MachineType.CloudRegion, environment.Name + "_CloudRegion");
                            CreateMachine(environment, MachineType.CloudRegion, "MachineTests_Machine1");
                            CreateMachine(environment, MachineType.CloudRegion, "MachineTests_Machine2");
                            CreateMachine(environment, MachineType.CloudRegion, "Unmodified_TestMachine");

                            CreateMachine(environment, MachineType.OfflineDrop, environment.Name + "_OfflineDrop");
                            CreateMachine(environment, MachineType.PollingTentacle, environment.Name + "_Polling");
                            CreateMachine(environment, MachineType.ListeningTentacle, environment.Name + "_Listening");

                            CreateAccount(environment, "Account_SSH", xAccountType.SSH);
                            CreateMachine(environment, MachineType.SSH, environment.Name + "_SSH");
                            break;
                        default:
                            CreateMachine(environment, MachineType.CloudRegion,environment.Name + "_CloudRegion");
                            break;
                    }
                }
            }
        }
        #endregion

        #region Lifecycle

        private static List<LifecycleResource> CreateLifecycles()
        {
            CreateEnvironments();//Start by creating the environments that will be added to the repository

            //Create the rest of the lifecycles for test
            var lifecycleList = new List<LifecycleResource>();

            foreach (var lifecycleName in LifecycleNames)
            {
                var lifecycle = _repository.Lifecycles.FindByName(lifecycleName).Result ??
                                new LifecycleResource();

                lifecycle.Name = lifecycleName;
                lifecycle.Description = GeneralResourceProperty.ResourceDescription;

                if (lifecycle.Name == "Default Lifecycle")
                {
                    lifecycle.Phases.Clear();

                    for (int i = 0; i < EnvironmentNames.Length; i++)
                    {
                        var environment = _repository.Environments.FindByName(EnvironmentNames[i]).Result;

                        var phase = new PhaseResource()
                        {
                            Name = environment.Name,
                            IsOptionalPhase = false,
                            OptionalDeploymentTargets = ReferenceCollection.One(environment.Id)
                        };

                        lifecycle.Phases.Insert(i, phase);
                    }
                }

                try
                {
                    Log.Information($"Creating/Modifying Lifecycle [{lifecycle.Name}]");
                    lifecycleList.Add(lifecycle.Id == null ?
                        _repository.Lifecycles.Create(lifecycle).Result :
                        _repository.Lifecycles.Modify(lifecycle).Result
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return lifecycleList;

        }

        private static void ConfigureLifecycle(LifecycleResource lifecycle)
        {
            //var lifecycle = _repository.Lifecycles.FindByName(lifecycleName);
        }

        #endregion

        #region Machines

        enum MachineType
        {
            CloudRegion,
            SSH,
            ListeningTentacle,
            PollingTentacle,
            OfflineDrop
        }
        private static void CreateMachine(EnvironmentResource environment, MachineType endpointType, string machineName, ReferenceCollection roles = null)
        {
            var machine = _repository.Machines.FindByName(machineName).Result ?? new MachineResource();

            switch (endpointType)
            {
                case MachineType.CloudRegion:
                    machine.Name = machineName;
                    machine.Endpoint = new CloudRegionEndpointResource();
                    break;
                case MachineType.OfflineDrop:
                    machine.Name = machineName;
                    machine.Endpoint = new OfflineDropEndpointResource()
                    {
                        ApplicationsDirectory = "c:\\path",
                        DropFolderPath = "c:\\path",
                        OctopusWorkingDirectory = "c:\\path"
                    };
                    break;
                case MachineType.ListeningTentacle:
                    machine.Name = machineName;
                    machine.Endpoint = new ListeningTentacleEndpointResource()
                    {
                        Uri = "https://localhost:1234",
                        Thumbprint = "1977BFA166FD8A9BAAAB4D0EF4DF798BE13A00FB"
                    };
                    break;
                case MachineType.SSH:
                    machine.Name = machineName;
                    machine.Endpoint = new SshEndpointResource()
                    {
                        Host = "host.com",
                        Port = 22,
                        AccountId =
                            (_repository.Accounts.FindOne(a => a.AccountType == AccountType.SshKeyPair).Result.Id),
                        Fingerprint = "04:f0:5e:05:c7:f3:84:34:e3:67:9a:5f:78:0c:78:ff"
                    };
                    break;
                case MachineType.PollingTentacle:
                    machine.Name = machineName;
                    machine.Endpoint = new PollingTentacleEndpointResource()
                    {
                        Uri = "poll://mrwr5ypwjz8tmr5b8z2x/",
                        Thumbprint = "1977BFA166FD8A9BAAAB4D0EF4DF798BE13A00FB"
                    };
                    break;
                default:
                    Log.Information("unknown type");
                    break;
            }

            machine.EnvironmentIds = new ReferenceCollection(environment.Id);
            machine.Roles = roles ?? new ReferenceCollection("default-role");

            try
            {
                Log.Information($"Creating/Modifying Machine [{machine.Name}]");

                if (machine.Id != null)
                    _repository.Machines.Modify(machine).Wait();
                else
                    _repository.Machines.Create(machine).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }
        #endregion

        #region Accounts

        enum xAccountType
        {
            SSH
        }
        private static void CreateAccount(EnvironmentResource environment, string accountName, xAccountType accountType)
        {
            var accountExists = _repository.Accounts.FindByName(accountName).Result;

            AccountResource account = null;

            switch (accountType)
            {
                case xAccountType.SSH:

                    account = new SshKeyPairAccountResource()
                    {
                        Id = accountExists?.Id,
                        Name = accountName,
                        EnvironmentIds = new ReferenceCollection(environment.Id),
                        PrivateKeyFile = new SensitiveValue() { HasValue = true, NewValue = "stuff" },
                        Description = GeneralResourceProperty.ResourceDescription,
                        Username = "Dio.Brando",
                        Links = accountExists?.Links
                    };
                    break;
                default:
                    break;
            }

            try
            {
                Log.Information($"Creating/Modifying Account [{account.Name}]");
                if (account.Id == null)
                    _repository.Accounts.Create(account).Wait();
                else
                    _repository.Accounts.Modify(account).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion

        #region Tenants and Tenant tag sets

        public static void CreateTagSets()
        {
            var tagSetList = new List<TagSetResource>();

            foreach (var tagSetName in TagSetNames)
            {
                //todo fix the tagset shit
                var tagSet = _repository.TagSets.FindByName(tagSetName).Result ?? new TagSetResource();

                tagSet.Name = tagSetName;
                tagSet.Description = GeneralResourceProperty.ResourceDescription;

                try
                {
                    Log.Information($"Creating/Modifying TagSet [{tagSet.Name}]");
                    tagSetList.Add(tagSet.Id == null
                        ? _repository.TagSets.Create(tagSet).Result
                        : _repository.TagSets.Modify(tagSet).Result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
        }

        public static void CreateTenants()
        {
            var tenantList = new List<TenantResource>();

            foreach (var tenantName in TenantNames)
            {
                var tenant = _repository.Tenants.FindByName(tenantName).Result ?? new TenantResource();

                tenant.Name = tenantName;

                try
                {
                    Log.Information($"Creating/Modifying Tenant [{tenant.Name}]");
                    tenantList.Add(tenant.Id == null
                        ? _repository.Tenants.Create(tenant).Result
                        : _repository.Tenants.Modify(tenant).Result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
        }

        #endregion
    }
}
