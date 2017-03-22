using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Octoposh.Cmdlets;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    /// <summary>
    /// This class converts regular Octopus objects into human-friendly Octoposh objects. This class contains 1 public method per GET-* cmdlet with the same name as the cmdlet.
    /// </summary>
    internal class OutputConverter
    {

        private static readonly OctopusConnection _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];

        private readonly ResourceCollector _resourceCollector = new ResourceCollector();

        //todo figure out how to make this work in a more generic way
        //private readonly ResourceCollector2 _resourceCollector2 = new ResourceCollector2(_connection);

        public List<OutputOctopusMachine> GetOctopusMachine (List<MachineResource> baseResources)
        {

            var list = new List<OutputOctopusMachine>();

            foreach (var machine in baseResources)
            {
                var environmentNames = new List<string>();

                foreach (var environmentId in machine.EnvironmentIds)
                {
                    if (_resourceCollector.Environments.Any(x => x.Id == environmentId))
                    {
                        environmentNames.Add(
                            (_resourceCollector.Environments.First(x => x.Id == environmentId)).Name);
                    }
                    else
                    {
                        _resourceCollector.Environments.Add(_connection.Repository.Environments.Get(environmentId));

                        environmentNames.Add((_resourceCollector.Environments.First(x => x.Id == environmentId)).Name);
                    }
                }

                var communicationStyle = "";

                switch (machine.Endpoint.GetType().ToString())
                {
                    case "Octopus.Client.Model.Endpoints.CloudRegionEndpointResource":
                        communicationStyle = "CloudRegion";
                        break;
                    case "Octopus.Client.Model.Endpoints.ListeningTentacleEndpointResource":
                        communicationStyle = "ListeningTentacle";
                        break;
                    case "Octopus.Client.Model.Endpoints.PollingTentacleEndpointResource":
                        communicationStyle = "PollingTentacle";
                        break;
                    case "Octopus.Client.Model.Endpoints.OfflineDropEndpointResource":
                        communicationStyle = "OfflineDrop";
                        break;

                    case "Octopus.Client.Model.Endpoints.SSHEndpointResource":
                        communicationStyle = "SSHEndpoint";
                        break;
                }

                list.Add(new OutputOctopusMachine
                {
                    Name = machine.Name,
                    Id = machine.Id,
                    EnvironmentName = environmentNames.ToArray(),
                    Resource = machine,
                    Uri = machine.Uri,
                    IsDisabled = machine.IsDisabled,
                    Status = machine.Status.ToString(),
                    StatusSummary = machine.StatusSummary,
                    Roles = machine.Roles,
                    HasLatestClamari = machine.HasLatestCalamari,
                    CommunicationStyle = communicationStyle
                });
            }

            return list;
        }

        public List<OutputOctopusEnvironment> GetOctopusEnvironment(List<EnvironmentResource> baseResources)
        {
            var list = new List<OutputOctopusEnvironment>();
            var dashboard = _connection.Repository.Dashboards.GetDashboard();

            foreach (var environment in baseResources)
            {
                var deployments = new List<OutputDiscreteDeployment>();

                var dashboardItems = dashboard.Items.Where(x => x.EnvironmentId == environment.Id);

                foreach (var item in dashboardItems)
                {
                    var deploymentevent = _connection.Repository.Events.List(regardingDocumentId: item.Id).Items.First();
                    var task = _connection.Repository.Tasks.Get(item.TaskId);

                    deployments.Add(new OutputDiscreteDeployment
                    {
                        ProjectName = dashboard.Projects.Find(x => x.Id == item.ProjectId).Name,
                        EnvironmentName = dashboard.Environments.Find(x => x.Id == item.EnvironmentId).Name,
                        Releaseversion = item.ReleaseVersion,
                        State = item.State.ToString(),
                        CreatedBy = deploymentevent.Username,
                        StartTime = task.StartTime?.DateTime,
                        Endtime = task.CompletedTime?.DateTime,
                    });
                }

                list.Add(new OutputOctopusEnvironment
                {
                    Name = environment.Name,
                    Id = environment.Id,
                    Deployments = deployments,
                    Machines = _connection.Repository.Environments.GetMachines(environment),
                    Resource = environment
                });
            }

            return list;
        }

        public List<OutputOctopusProject> GetOctopusProject(List<ProjectResource> baseResourceList)
        {
            var list = new List<OutputOctopusProject>();
            var dashboard = _connection.Repository.Dashboards.GetDashboard();

            foreach (var project in baseResourceList)
            {
                var deployments = new List<OutputDiscreteDeployment>();
                var dashboardItems = dashboard.Items.Where(x => x.ProjectId == project.Id);
                
                foreach (var item in dashboardItems)
                {
                    var deploymentevent = _connection.Repository.Events.List(regardingDocumentId: item.Id).Items.First();

                    deployments.Add(new OutputDiscreteDeployment
                    {
                        ProjectName = dashboard.Projects.Find(x => x.Id == item.ProjectId).Name,
                        EnvironmentName = dashboard.Environments.Find(x => x.Id == item.EnvironmentId).Name,
                        Releaseversion = item.ReleaseVersion,
                        State = item.State.ToString(),
                        CreatedBy = deploymentevent.Username,
                        StartTime = item.Created.DateTime,
                        Endtime = item.CompletedTime?.DateTime,
                    });
                }

                LifecycleResource lifecycle;
                if (_resourceCollector.IsResourceHereAlready(project.LifecycleId))
                {
                    lifecycle = _resourceCollector.Lifecycles.First(x => x.Id == project.LifecycleId);
                }
                else
                {
                    lifecycle = _connection.Repository.Lifecycles.Get(project.LifecycleId);
                    _resourceCollector.Lifecycles.Add(lifecycle);
                }

                list.Add(new OutputOctopusProject
                {
                    Name = project.Name,
                    Id = project.Id,
                    ProjectGroupName = dashboard.ProjectGroups.First(x => x.Id == project.ProjectGroupId).Name,
                    LifecycleName = lifecycle.Name,
                    LatestDeployments = deployments,
                    AutoCreateRelease = project.AutoCreateRelease,
                    Resource = project
                });
            }

            return list;
        }

        public List<OutputOctopusProjectGroup> GetOctopusProjectGroup(List<ProjectGroupResource> baseResourceList)
        {
            var list = new List<OutputOctopusProjectGroup>();

            foreach (var projectGroup in baseResourceList)
            {
                var projectNames = new List<string>();

                var projects = _connection.Repository.ProjectGroups.GetProjects(projectGroup);

                if (projects.Count > 0)
                {
                    foreach (var project in projects)
                    {
                        projectNames.Add(project.Name);
                    }
                }

                list.Add(new OutputOctopusProjectGroup()
                {
                    Name = projectGroup.Name,
                    Id = projectGroup.Id,
                    Projects = projectNames,
                    Resource = projectGroup
                });
            }

            return list;
        }

        public List<OutputOctopusRelease> GetOctopusRelease(List<ReleaseResource> baseResourceList)
        {
            var list = new List<OutputOctopusRelease>();

            var allProjects = _connection.Repository.Projects.FindAll();

            foreach (var release in baseResourceList)
            {
                var deployments = _connection.Repository.Releases.GetDeployments(release, 0).Items.ToList();

                var releaseCreateEvent = GetCreateEvent(release.Id);

                list.Add(new OutputOctopusRelease()
                {
                    ProjectName = allProjects.FirstOrDefault(x => x.Id == release.ProjectId)?.Name,
                    Id = release.Id,
                    ReleaseVersion = release.Version,
                    ReleaseNotes = release.ReleaseNotes,
                    CreationDate = release.Assembled.DateTime,
                    CreatedBy = releaseCreateEvent?.Username,
                    Deployments = deployments,
                    Resource = release
                });
            }

            return list;

        }

        public List<OutputOctopusDashboardEntry> GetOctopusDashboard(DashboardResource rawDashboard, string[] projectName, string[] environmentName, string[] deploymentStatus)
        {
            var list = new List<OutputOctopusDashboardEntry>();
            var environments = new List<DashboardEnvironmentResource>();
            var projects = new List<DashboardProjectResource>();

            environments = environmentName != null ? rawDashboard.Environments.Where(e => environmentName.Any(en => String.Equals(en, e.Name, StringComparison.CurrentCultureIgnoreCase))).ToList() : rawDashboard.Environments;

            projects = projectName != null ? rawDashboard.Projects.Where(p => projectName.Any(pn => String.Equals(pn, p.Name, StringComparison.CurrentCultureIgnoreCase))).ToList() : rawDashboard.Projects;
            
            foreach (var deployment in rawDashboard.Items)
            {
                var project = projects.FirstOrDefault(p => p.Id == deployment.ProjectId);
                var environment = environments.FirstOrDefault(e => e.Id == deployment.EnvironmentId);

                if (deploymentStatus != null && !deploymentStatus.Contains(deployment.State.ToString()))
                {
                    continue;
                }
                
                if (projects.All(p => p.Id != deployment.ProjectId))
                {
                    continue;
                }
                
                if (environments.All(e => e.Id != deployment.EnvironmentId))
                {
                    continue;
                }

                DateTime endDate;
                string duration;

                if (deployment.CompletedTime != null)
                {
                    TimeSpan? durationSpan = deployment.CompletedTime - deployment.Created;
                    duration = string.Format("{0:D2}:{1:D2}:{2:D2}", durationSpan.Value.Hours,durationSpan.Value.Minutes, durationSpan.Value.Seconds);
                    endDate = deployment.CompletedTime.Value.DateTime;
                }
                else
                {
                    endDate = DateTime.Now.Date;
                    TimeSpan? durationSpan = endDate - deployment.Created;
                    duration = string.Format("{0:D2}:{1:D2}:{2:D2}", durationSpan.Value.Hours,durationSpan.Value.Minutes, durationSpan.Value.Seconds);
                }

                list.Add(new OutputOctopusDashboardEntry()
                {
                    ProjectName = project.Name,
                    EnvironmentName = environment.Name,
                    ReleaseVersion = deployment.ReleaseVersion,
                    DeploymentStatus = deployment.State.ToString(),
                    StartDate = deployment.QueueTime.DateTime,
                    EndDate = endDate,
                    Duration = duration,
                    IsCompleted = deployment.IsCompleted,
                    HasPendingInterruptions = deployment.HasPendingInterruptions,
                    HasWarninsOrErrors = deployment.HasWarningsOrErrors
                });

            }
            return list;
        }

        public List<OutputOctopusDeployment> GetOctopusDeployment(List<DeploymentResource> baseResourceList, List<DashboardEnvironmentResource> environments, List<DashboardProjectResource> projects, List<ReleaseResource> releases)
        {
            var list = new List<OutputOctopusDeployment>();

            foreach (var deployment in baseResourceList)
            {
                var project = projects.FirstOrDefault(p => p.Id == deployment.ProjectId);
                var environment = environments.FirstOrDefault(e => e.Id == deployment.EnvironmentId);
                var task = _connection.Repository.Tasks.Get(deployment.Links["Task"]);
                var deploymentCreateEvent = GetCreateEvent(deployment.Id);
                var release = releases.FirstOrDefault(r => r.Id == deployment.ReleaseId);
                var releaseCreateEvent = GetCreateEvent(release.Id);
                var deploymentProcess = _connection.Repository.DeploymentProcesses.Get(release.Links["ProjectDeploymentProcessSnapshot"]);

                string duration;

                if (task.CompletedTime != null)
                {
                    TimeSpan? durationSpan = task.CompletedTime.Value - deployment.Created;
                    duration = string.Format("{0:D2}:{1:D2}:{2:D2}", durationSpan.Value.Hours,
                        durationSpan.Value.Minutes, durationSpan.Value.Seconds);
                }
                else
                {
                    var endDate = DateTime.Now.Date;
                    TimeSpan? durationSpan = endDate - deployment.Created;
                    duration = string.Format("{0:D2}:{1:D2}:{2:D2}", durationSpan.Value.Hours,
                        durationSpan.Value.Minutes, durationSpan.Value.Seconds);
                }

                var packages = new List<OutputDeploymentPackage>();

                if(release.SelectedPackages.Count > 0) {
                    foreach (var package in release.SelectedPackages)
                    {
                        var packageId =
                            deploymentProcess.Steps.FirstOrDefault(s => s.Actions.Any(a => a.Name == package.StepName)).Actions.FirstOrDefault(a => a.Name == package.StepName).Properties["Octopus.Action.Package.PackageId"].Value;

                        packages.Add(new OutputDeploymentPackage()
                        {
                            Id = packageId,
                            Version = package.Version,
                            StepName = package.StepName
                        });
                    }
                }

                list.Add(new OutputOctopusDeployment()
                {
                    ProjectName = project.Name,
                    EnvironmentName = environment.Name,
                    DeploymentStartTime = task.StartTime.Value.DateTime,
                    DeploymentEndTime = task.CompletedTime.Value.DateTime,
                    DeploymentStartedBy = deploymentCreateEvent?.Username,
                    Id = deployment.Id,
                    Duration = duration,
                    Status = task.State.ToString(),
                    ReleaseVersion = release.Version,
                    ReleaseCreationDate = release.Assembled.DateTime,
                    ReleaseNotes = release.ReleaseNotes,
                    ReleaseCreatedBy = releaseCreateEvent.Username,
                    Packages = packages,
                    Resource = deployment,
                });
            }

            return list;
        }

        public List<OutputOctopusTeam> GetOctopusTeam(List<TeamResource> baseResourceList)
        {
            var list = new List<OutputOctopusTeam>();

            var dashboard = _connection.Repository.Dashboards.GetDashboard();
            var allRoles = _connection.Repository.UserRoles.FindAll();

            foreach (var team in baseResourceList)
            {
                var roles = allRoles.Where(r => team.UserRoleIds.Contains(r.Id)).Select(r => r.Name).ToList();
                var projects = dashboard.Projects.Where(p => team.ProjectIds.Contains(p.Id)).Select(p => p.Name).ToList();
                var environments = dashboard.Environments.Where(e => team.EnvironmentIds.Contains(e.Id)).Select(e => e.Name).ToList();
                var projectGroups = new List<string>();
                var tenants = new List<string>();

                if(team.TenantIds.Count > 0) { 
                    foreach (var tenantId in team.TenantIds)
                    {
                        if (_resourceCollector.Tenants.Any(x => x.Id == tenantId))
                        {
                            tenants.Add(
                                (_resourceCollector.Tenants.First(x => x.Id == tenantId)).Name);
                        }
                        else
                        {
                            var tenant = _connection.Repository.Tenants.Get(tenantId);

                            _resourceCollector.Tenants.Add(tenant);

                            tenants.Add(tenant.Name);
                        }
                    }
                }

                list.Add(new OutputOctopusTeam()
                {
                    Name = team.Name,
                    Id =  team.Id,
                    Roles = roles,
                    Projects = projects,
                    Environments = environments,
                    ProjectGroups = projectGroups,
                    Tenants = tenants,
                    Resource = team
                });
            }

            return list;
        }

        public List<OutputOctopusUser> GetOctopusUser (List<UserResource> baseResourceList)
        {
            var list = new List<OutputOctopusUser>();

            var allTeams = _connection.Repository.Teams.FindAll();

            foreach (var user in baseResourceList)
            {
                var teams = allTeams.Where(t => t.MemberUserIds.Contains(user.Id)).Select(t => t.Name).ToList();
                
                list.Add(new OutputOctopusUser()
                {
                    UserName = user.Username,
                    DisplayName = user.DisplayName,
                    EmailAdress = user.EmailAddress,
                    Teams = teams,
                    IsService = user.IsService,
                    Resource = user
                });
            }

            return list;
        }

        public List<OutputOctopusLifecycle> GetOctopusLifecycle(List<LifecycleResource> baseResourceList)
        {
            var list = new List<OutputOctopusLifecycle>();

            foreach (var lifecycle in baseResourceList)
            {
                list.Add(new OutputOctopusLifecycle()
                {
                    Name = lifecycle.Name,
                    Id = lifecycle.Id,
                    Description = lifecycle.Description,
                    Phases = lifecycle.Phases,
                    ReleaseRetentionPolicy = lifecycle.ReleaseRetentionPolicy,
                    TentacleRetentionPolicy = lifecycle.TentacleRetentionPolicy,
                    LastModifiedOn = lifecycle.LastModifiedOn?.DateTime,
                    LastModifiedBy = lifecycle.LastModifiedBy,
                    Resource = lifecycle
                });
            }

            return list;
        }

        public List<OutputOctopusFeed> GetOctopusFeed(List<FeedResource> baseResourceList)
        {
            var list = new List<OutputOctopusFeed>();

            foreach (var feed in baseResourceList)
            {
                list.Add(new OutputOctopusFeed()
                {
                    Name = feed.Name,
                    FeedURI = feed.FeedUri,
                    FeedType = feed.FeedType,
                    LoginUser = feed.Username,
                    Id = feed.Id,
                    Resource = feed
                });
            }

            return list;
        }

        public List<OutputOctopusChannel> GetOctopusChannel(List<ChannelResource> baseResourceList)
        {
            var list = new List<OutputOctopusChannel>();

            foreach (var channel in baseResourceList)
            {

                ProjectResource project;
                LifecycleResource lifecycleName;

                if (_resourceCollector.Projects.Any(x => x.Id == channel.ProjectId))
                {
                    project = _resourceCollector.Projects.First(x => x.Id == channel.ProjectId);
                }
                else
                {
                    _resourceCollector.Projects.Add(_connection.Repository.Projects.Get(channel.ProjectId));

                    project = _resourceCollector.Projects.First(x => x.Id == channel.ProjectId);
                }

                string lifecycleId = null;

                lifecycleId = channel.LifecycleId ?? project.LifecycleId;

                if (_resourceCollector.Lifecycles.Any(x => x.Id == lifecycleId))
                {
                    lifecycleName = _resourceCollector.Lifecycles.First(x => x.Id == lifecycleId);
                }
                else
                {
                    _resourceCollector.Lifecycles.Add(_connection.Repository.Lifecycles.Get(lifecycleId));

                    lifecycleName = _resourceCollector.Lifecycles.First(x => x.Id == lifecycleId);
                }

                list.Add(new OutputOctopusChannel()
                {
                    Name   = channel.Name,
                    Id = channel.Id,
                    Description = channel.Description,
                    ProjectName = project.Name,
                    LifecycleName = lifecycleName.Name,
                    IsDefault = channel.IsDefault,
                    TenantTags = channel.TenantTags,
                    Rules = channel.Rules,
                    Resource = channel,
                    
                });
            }

            return list;
        }

        public List<OutputOctopusTenant> GetOctopusTenant(List<TenantResource> baseResourceList)
        {
            var list = new List<OutputOctopusTenant>();

            var dashboard = _connection.Repository.Dashboards.GetDashboard();

            foreach (var tenant in baseResourceList)
            {
                list.Add(new OutputOctopusTenant()
                {
                    Name = tenant.Name,
                    Id = tenant.Id,
                    TagSets = ToFriendlyTags(tenant.TenantTags),
                    ProjectEnvironments = ToFriendlyProjectEnvironments(tenant.ProjectEnvironments,dashboard),
                    Resource = tenant
                });
            }

            return list;
        }

        public List<OutputOctopusTagSet> GetOctopusTagSet(List<TagSetResource> baseResourceList)
        {
            var list = new List<OutputOctopusTagSet>();

            foreach (var tagSet in baseResourceList)
            {
                list.Add(new OutputOctopusTagSet()
                {
                    Name = tagSet.Name,
                    Id = tagSet.Id,
                    Description = tagSet.Description,
                    SortOrder = tagSet.SortOrder,
                    Tags = tagSet.Tags,
                    Resource = tagSet
                });
            }
            return list;
        }
        
        public List<OutputOctopusVariableSet> GetOctopusVariableSet(List<VariableSetResource> baseResourceList, List<ProjectResource> projectList, List<LibraryVariableSetResource> libraryVariableSetList, SwitchParameter includeUsage)
        {
            var list =  new List<OutputOctopusVariableSet>();
            var allProjects = new List<ProjectResource>();

            if (includeUsage)
            {
                allProjects = _connection.Repository.Projects.FindAll();
            }

            foreach (var variableSet in baseResourceList)
            {

                var friendlyVariables = ToFriendlyVariables(variableSet.Variables, variableSet.ScopeValues);

                var variableSetOwner = new VariableSetOwner();
                var usage = new List<string>();

                switch (variableSet.OwnerId.Split('-')[0])
                {
                    case "Projects":
                        variableSetOwner.Type = OwnerType.Project;
                        variableSetOwner.Name = projectList.FirstOrDefault(p => variableSet.OwnerId == p.Id)?.Name;
                        break;
                    case "LibraryVariableSets":
                        variableSetOwner.Type = OwnerType.LibraryVariableSet;
                        variableSetOwner.Name = libraryVariableSetList.FirstOrDefault(lvs => variableSet.OwnerId == lvs.Id)?.Name;
                        break;
                }

                if (includeUsage && variableSetOwner.Type == OwnerType.LibraryVariableSet)
                {
                    usage.AddRange(allProjects.Where(p => p.IncludedLibraryVariableSetIds.Contains(variableSet.OwnerId)).Select(p => p.Name).ToList());
                }

                list.Add(new OutputOctopusVariableSet()
                {
                    ProjectName = (variableSetOwner.Type == OwnerType.Project) ? variableSetOwner.Name : null,
                    LibraryVariableSetName = (variableSetOwner.Type == OwnerType.LibraryVariableSet) ? variableSetOwner.Name : null,
                    Usage = usage,
                    ID = variableSet.Id,
                    Variables = friendlyVariables,
                    LastModifiedOn = variableSet.LastModifiedOn.GetValueOrDefault().DateTime,
                    LastModifiedBy = variableSet.LastModifiedBy,
                    Resource = variableSet
                }); 
            }

            return list;
        }

        private IDictionary<string, List<string>> ToFriendlyProjectEnvironments(IDictionary<string, ReferenceCollection> tenantProjectEnvironments, DashboardResource dashboardResource)
        {
            var friendlyProjectEnvironments = new Dictionary<string, List<string>>();

            var dashboard = dashboardResource;

            foreach (var tPE in tenantProjectEnvironments)
            {
                var projectName = dashboard.Projects.FirstOrDefault(x => x.Id == tPE.Key)?.Name;
                var environmentNames = new List<string>();

                environmentNames.AddRange(dashboard.Environments.Where(e => tPE.Value.Contains(e.Id)).Select(e => e.Name).ToList());

                friendlyProjectEnvironments.Add(projectName, environmentNames);
            }

            return friendlyProjectEnvironments;
        }

        private IDictionary<string, List<string>> ToFriendlyTags(ReferenceCollection tenantTags)
        {
            var tagSets = new Dictionary<string, List<string>>();

            foreach (var rawTag in tenantTags)
            {
                // rawTags come in strings like "MyTagSet/MyTag". So we are splitting them here to get the separated values.
                //[0] will be the TagSet and [1] will be the tag
                var splitTag = rawTag.Split('/');
                var tagset = splitTag[0];
                var tag = splitTag[1];

                if (!tagSets.ContainsKey(tagset))
                {
                    tagSets.Add(tagset, new List<string>() { tag });
                }
                else
                {
                    tagSets[tagset].Add(tag);
                }
            }

            return tagSets;
        }

        private List<FriendlyVariable> ToFriendlyVariables(IList<VariableResource> Variables, VariableScopeValues ScopeValues)
        {
            var list = new List<FriendlyVariable>();

            foreach (var variable in Variables)
            {
                var scope = ToFriendlyScopeCollection(variable.Scope, ScopeValues);

                list.Add(new FriendlyVariable()
                {
                    Name = variable.Name,
                    Value = variable.Name,
                    IsEditable = variable.IsEditable,
                    IsSensitive = variable.IsSensitive,
                    Scope = scope,
                    Prompt = variable.Prompt
                });
            }

            return list;
        }

        private FriendlyScopeCollection ToFriendlyScopeCollection(ScopeSpecification variableScope, VariableScopeValues scopeValues)
        {
            var scopeCollection = new FriendlyScopeCollection();
            
            foreach (var scope in variableScope)
            {
                switch (scope.Key.ToString())
                {
                    case "Machine":
                        scopeCollection.Machines = scopeValues.Machines.Where(m => scope.Value.Contains(m.Id)).Select(m => m.Name).ToList();
                        break;
                    case "Environment":
                        scopeCollection.Environments = scopeValues.Environments.Where(e => scope.Value.Contains(e.Id)).Select(e => e.Name).ToList();
                        break;
                    case "Action":
                        scopeCollection.Actions = scopeValues.Actions.Where(a => scope.Value.Contains(a.Id)).Select(a => a.Name).ToList();
                        break;
                    case "Role":
                        scopeCollection.Roles = scopeValues.Roles.Where(r => scope.Value.Contains(r.Id)).Select(r => r.Name).ToList();
                        break;
                    case "Channel":
                        scopeCollection.Channels = scopeValues.Channels.Where(c => scope.Value.Contains(c.Id)).Select(c => c.Name).ToList();
                        break;
                }
            }

            return scopeCollection;
        }

        private EventResource GetCreateEvent(string resourceId)
        {
            return _connection.Repository.Events.List(0, null, resourceId, true).Items.LastOrDefault();
        }
    }
}
