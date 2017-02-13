using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Octoposh.Cmdlets;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    class OutputConverter
    {

        private readonly ResourceCollector _resourceCollector = new ResourceCollector();

        private readonly OctopusConnection _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];

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

                var releaseCreateEvent = _connection.Repository.Events.List(0, null, release.Id, true).Items.Last();

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

        public List<OutputOctopusDashboardEntry> GetOctopusDashboard(DashboardResource rawDashboard, List<string> projectName, List<string> environmentName, List<string> deploymentStatus)
        {
            var list = new List<OutputOctopusDashboardEntry>();

            if(environmentName.Count > 0) { 
                rawDashboard.Environments = rawDashboard.Environments.Where(e => environmentName.Contains(e.Name)).ToList();
            }

            if(projectName.Count > 0) { 
                rawDashboard.Projects = rawDashboard.Projects.Where(p => projectName.Contains(p.Name)).ToList();
            }

            foreach (var deployment in rawDashboard.Items)
            {
                //filtering by deployment status
                if (deploymentStatus.Contains(deployment.State.ToString()))
                {
                    var project = rawDashboard.Projects.FirstOrDefault(p => p.Id == deployment.ProjectId);
                    var environment = rawDashboard.Environments.FirstOrDefault(e => e.Id == deployment.EnvironmentId);

                    //filtering by project and environment
                    if (project != null && environment != null)
                    {
                        DateTime endDate;
                        string duration;

                        if (deployment.CompletedTime != null)
                        {
                            TimeSpan? durationSpan = deployment.CompletedTime - deployment.Created;
                            duration = string.Format("{0:D2}:{1:D2}:{2:D2}", durationSpan.Value.Hours,
                                durationSpan.Value.Minutes, durationSpan.Value.Seconds);
                            endDate = deployment.CompletedTime.Value.DateTime;
                        }
                        else
                        {
                            endDate = DateTime.Now.Date;
                            TimeSpan? durationSpan = endDate - deployment.Created;
                            duration = string.Format("{0:D2}:{1:D2}:{2:D2}", durationSpan.Value.Hours,
                                durationSpan.Value.Minutes, durationSpan.Value.Seconds);
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
                }
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
                var deploymentCreateEvent = _connection.Repository.Events.List(0, null, deployment.Id,true).Items.LastOrDefault();
                var release = releases.FirstOrDefault(r => r.Id == deployment.ReleaseId);
                var releaseCreateEvent = _connection.Repository.Events.List(0, null, release.Id, true).Items.LastOrDefault();
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
    }
}
