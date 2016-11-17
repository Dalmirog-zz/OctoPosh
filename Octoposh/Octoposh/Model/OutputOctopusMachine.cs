using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    internal class OutputOctopusMachine
    {
        public string Name { get; set; }
        public string[] EnvironmentName { get; set; }
        public string Id { get; set; }
        public string Uri { get; set; }
        public bool IsDisabled { get; set; }
        public ReferenceCollection Roles { get; set; }
        public bool HasLatestClamari { get; set; }
        public string CommunicationStyle { get; set; }
        public MachineResource Resource { get; set; }
    }
}
