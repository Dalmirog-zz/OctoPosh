using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    class OutputOctopusProject
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string ProjectGroupName { get; set; }
        public string LifecycleName { get; set; }
        public List<OutputDiscreteDeployment> LatestDeployments { get; set; }
        public bool AutoCreateRelease { get; set; }
        public ProjectResource Resource { get; set; }
    }
}
