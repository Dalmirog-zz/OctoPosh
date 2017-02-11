using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusTeam
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Environments { get; set; }
        public List<string> Projects { get; set; }
        public List<string> ProjectGroups { get; set; }
        public List<string> Tenants { get; set; }
        public TeamResource Resource { get; set; }
    }
}