using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusRelease
    {
        public string ProjectName { get; set; }
        public string Id { get; set; }
        public string ReleaseVersion { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public ReleaseResource Resource { get; set; }
        public List<DeploymentResource> Deployments { get; set; }
        public List<OutputDeploymentPackage> Packages { get; set; }
    }
}
