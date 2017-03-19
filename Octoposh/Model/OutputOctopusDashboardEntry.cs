using System;

namespace Octoposh.Model
{
    public class OutputOctopusDashboardEntry
    {
        public string ProjectName { get; set; }
        public string EnvironmentName { get; set; }
        public string ReleaseVersion { get; set; }
        public string DeploymentStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasPendingInterruptions { get; set; }
        public bool HasWarninsOrErrors { get; set; }
        public string Duration { get; set; }
    }
}
