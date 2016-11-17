using System.Collections.Generic;
using System.Linq;
using Octoposh.Cmdlets;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    class OutputConverter
    {
        private ResourceCollector _resourceCollector = new ResourceCollector();
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

                var environments =
                    _connection.Repository.Environments.GetAll().Where(x => machine.EnvironmentIds.Contains(x.Id));

                foreach (var environment in environments)
                {
                    environmentNames.Add(environment.Name);
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
                    Roles = machine.Roles,
                    HasLatestClamari = machine.HasLatestCalamari,
                    CommunicationStyle = communicationStyle
                });
            }

            return list;
        }
    }
}
