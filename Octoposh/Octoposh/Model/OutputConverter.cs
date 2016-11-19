using System;
using System.Collections.Generic;
using System.Linq;
using Octoposh.Cmdlets;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    class OutputConverter
    {

        private readonly ResourceCollector _resourceCollector = new ResourceCollector();

        private readonly OctopusConnection _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];

        /// <summary>
        /// Uses Octopus Resources to return objects with meaningful human-reabable properties.
        /// </summary>
        /// <param name="baseResources"></param>
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
                        communicationStyle = "PollingTentacle";
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

        /// <summary>
        /// Uses Octopus Resources to return objects with meaningful human-reabable properties.
        /// </summary>
        /// <param name="baseResources"></param>
        public List<OutputOctopusEnvironment> GetOctopusEnvironment(List<EnvironmentResource> baseResources)
        {
            var list = new List<OutputOctopusEnvironment>();
            var dashboard = _connection.Repository.Dashboards.GetDashboard();

            foreach (var environment in baseResources)
            {
                var deployments = new List<OutputDashboardDeployment>();

                var items = dashboard.Items.Where(x => x.EnvironmentId == environment.Id);

                foreach (var item in items)
                {
                    var deploymentevent = _connection.Repository.Events.List(regardingDocumentId: item.Id).Items.First();
                    var task = _connection.Repository.Tasks.Get(item.TaskId);

                    deployments.Add(new OutputDashboardDeployment
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
    }
}
