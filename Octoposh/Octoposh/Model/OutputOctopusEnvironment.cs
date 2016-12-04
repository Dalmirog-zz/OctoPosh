using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    internal class OutputOctopusEnvironment
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<MachineResource> Machines { get; set; }
        public List< OutputDiscreteDeployment> Deployments { get; set; }
        public EnvironmentResource Resource { get; set; }
    }
}
