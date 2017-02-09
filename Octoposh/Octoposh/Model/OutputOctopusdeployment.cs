using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusDeployment
    {
        public string ProjectName { get; set; }
        public string EnvironmentName { get; set; }
        public DateTime DeploymentStartTime { get; set; }
        public DateTime? DeploymentEndTime { get; set; }
        public string DeploymentStartedBy { get; set; }
        public string Id { get; set; }
        public string Duration { get; set; }
        public string Status { get; set; }
        public string ReleaseVersion { get; set; }
        public DateTime ReleaseCreationDate { get; set; }
        public string ReleaseNotes { get; set; }
        public string ReleaseCreatedBy { get; set; }
        public List<OutputDeploymentPackage> Packages { get; set; }
        public DeploymentResource Resource { get; set; }
    }
}