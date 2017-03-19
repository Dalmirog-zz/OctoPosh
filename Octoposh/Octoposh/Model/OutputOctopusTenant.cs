using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusTenant
    {
        public string Name { get; set; }
        public string Id { get; internal set; }
        public IDictionary<string, List<string>> TagSets { get; internal set; }
        public IDictionary<string, List<string>> ProjectEnvironments { get; set; }
        public TenantResource Resource { get; set; }
    }
}